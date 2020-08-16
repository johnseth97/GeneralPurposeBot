<template>
    <v-content>
        <section id="hero">
            <v-row no-gutters>
                <div class="header"
                     :style="'min-height: calc(45vh - ' + $vuetify.application.top + 'px)'">
                    <v-theme-provider dark>
                        <v-container fill-height>
                            <v-row align="center"
                                   class="white--text mx-auto"
                                   justify="center">
                                <v-col class="white--text text-left"
                                       cols="12"
                                       tag="h1">
                                    <span :class="[$vuetify.breakpoint.smAndDown ? 'display-1' : 'display-2']"
                                          class="font-weight-light">
                                        SAY HELLO TO
                                    </span>
                                    <br />
                                    <span :class="[$vuetify.breakpoint.smAndDown ? 'display-3': 'display-4']"
                                          class="font-weight-black">
                                        DesoBot
                                    </span>
                                    <br />
                                    <v-btn class="align-self-end"
                                           outlined
                                           large
                                           :href="`https://discordapp.com/oauth2/authorize?client_id=${$store.state.botInfo.clientId}&scope=bot&permissions=286379024`">
                                        <v-icon>mdi-plus</v-icon>Add to Server
                                    </v-btn>
                                </v-col>
                            </v-row>
                        </v-container>
                    </v-theme-provider>
                </div>
            </v-row>
        </section>

        <section id="bot-about">
            <div class="py-6"></div>
            <v-container class="text-center">
                <h2 class="display-2 font-weight-bold mb-3">About the Bot</h2>
                <v-responsive class="mx-auto title font-weight-light mb-8">
                    DesoBot is a Discord bot written for small and large servers alike. It's still being developed, with new features being added frequently.
                </v-responsive>
            </v-container>
            <div class="py-6"></div>
        </section>

        <section id="features"
                 class="blue darken-3">
            <div class="py-6"></div>

            <v-container class="text-center">
                <v-row>
                    <v-col v-for="({ icon, title, text }, i) in features"
                           :key="i"
                           cols="12"
                           md="4">
                        <v-card class="py-12 px-4"
                                flat>
                            <v-theme-provider dark>
                                <div>
                                    <v-avatar color="secondary"
                                              size="88">
                                        <v-icon large
                                                v-text="icon"></v-icon>
                                    </v-avatar>
                                </div>
                            </v-theme-provider>

                            <v-card-title class="justify-center font-weight-black text-uppercase"
                                          v-text="title"></v-card-title>

                            <v-card-text class="subtitle-1"
                                         v-text="text">
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
                <div class="py-6"></div>
            </v-container>
        </section>
    </v-content>
</template>

<script>
    export default {
        name: 'Home',
        data() {
            return {
                features: [
                    {
                        icon: 'mdi-account-voice',
                        title: 'Temporary Voice Chats',
                        text: 'This bot has the ability to create temporary voice chats. All a user has to do is join a special voice channel ' +
                            'in your server, and the bot will automatically create a new VC and move the user to it.Once a user is in the voice chat, ' +
                            'they have the ability to make it private and restrict who is able to join it, all through commands.'
                    },
                    {
                        icon: 'mdi-palette',
                        title: 'Color Roles',
                        text: 'The bot can also allow users to set the color of their own name with a simple command, automatically creating a role ' +
                            'for the user and adding it to them.Why limit the user to a small set of colors when they can pick from over sixteen million? '
                    },
                    {
                        icon: 'mdi-github',
                        title: 'Open Source',
                        text: "Bot doesn't do what you want? Want to know how a feature works? Want to just host it yourself? The entire bot is open-source " +
                            "- including the web interface for it. Written in C#, the bot can run almost anywhere (we run it in production on Linux under Docker), " +
                            "and the only external dependency is a MySQL or MariaDB database."
                    }
                ]
            }
        }
    }
</script>

<style scoped>
    .header {
        width: 100%;
        background: rgb(66,66,66);
        background: linear-gradient(90deg, rgba(66,66,66,1) 0%, rgba(66,66,66,1) 30%, rgba(62,62,130,1) 70%, rgba(62,62,130,1) 100%);
    }
</style>