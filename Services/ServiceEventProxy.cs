using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public abstract class ServiceEventProxy
    {
        public DiscordShardedClient Client;
        public ServerPropertiesService SpService;

        /// <summary>
        /// Installs event listeners for type `t`
        /// </summary>
        /// <param name="t">Type to look for events on</param>
        /// <param name="parentModuleName">Module name to use for checking enabled/disabled status</param>
        public void InstallEventListeners(Type t, string parentModuleName = null, string serviceDesc = "No description set")
        {
            if (parentModuleName != null)
            {
                if (SpService == null) throw new Exception("Server properties service wasn't set while module name was set!");
                SpService.DisableableServices.Add(parentModuleName, serviceDesc);
            }
            parentModuleName = "service." + parentModuleName;

            var methods = t.GetMethods().Where(method => method.GetCustomAttributes(typeof(EventListenerAttribute), false).Length > 0);
            foreach (var method in methods)
            {
                var attrib = method.GetCustomAttributes(typeof(EventListenerAttribute), false)[0] as EventListenerAttribute;
                switch (attrib.Event)
                {
                    case Event.UserVoiceStateUpdated:
                        Client.UserVoiceStateUpdated += (user, before, after) =>
                        {
                            if (parentModuleName == null)
                                return method.Invoke(this, new object[] { user, before, after }) as Task;
                            // please never copy this code for any purpose. like, really, there's probably a better way to do what you need to do.
                            if (before.VoiceChannel != null && !SpService.IsModuleEnabled(parentModuleName, before.VoiceChannel.Guild.Id))
                            {
                                var field = before.GetType().GetRuntimeFields()
                                    .FirstOrDefault(f => Regex.IsMatch(f.Name, $"\\A<{nameof(before.VoiceChannel)}>k__BackingField\\Z"));
                                field.SetValue(before, null);
                            }
                            if (after.VoiceChannel != null && !SpService.IsModuleEnabled(parentModuleName, after.VoiceChannel.Guild.Id))
                            {
                                var field = after.GetType().GetRuntimeFields()
                                    .FirstOrDefault(f => Regex.IsMatch(f.Name, $"\\A<{nameof(after.VoiceChannel)}>k__BackingField\\Z"));
                                field.SetValue(after, null);
                            }
                            if (before.VoiceChannel == null && after.VoiceChannel == null) return Task.CompletedTask;

                            return method.Invoke(this, new object[] { user, before, after }) as Task;
                        };
                        break;
                    case Event.MessageReceived:
                        Client.MessageReceived += (message) =>
                        {
                            if (parentModuleName == null || !(message.Channel is IGuildChannel))
                                return method.Invoke(this, new object[] { message }) as Task;
                            if (!SpService.IsModuleEnabled(parentModuleName, ((IGuildChannel)message.Channel).GuildId))
                                return Task.CompletedTask;
                            return method.Invoke(this, new object[] { message }) as Task;
                        };
                        break;
                    case Event.MessageUpdated:
                        Client.MessageUpdated += (cachedMessage, message, channel) =>
                        {
                            if (parentModuleName == null || !(channel is IGuildChannel))
                                return method.Invoke(this, new object[] { cachedMessage, message, channel }) as Task;
                            if (!SpService.IsModuleEnabled(parentModuleName, ((IGuildChannel)channel).GuildId))
                                return Task.CompletedTask;
                            return method.Invoke(this, new object[] { cachedMessage, message, channel }) as Task;
                        };
                        break;
                }
            }
        }
    }
}
