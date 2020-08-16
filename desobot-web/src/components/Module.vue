<template>
    <v-expansion-panel>
        <v-expansion-panel-header>
            <b>{{module.name}}</b>
            <v-spacer />
            {{module.summary}}
        </v-expansion-panel-header>
        <v-expansion-panel-content>
            <p v-if="module.remarks != null">{{module.remarks}}</p>
            <div v-if="module.submodules != null">
                <h3>Submodules</h3>
                <v-expansion-panels>
                    <Module v-for="(submodule, idx) in module.submodules" :key="idx" :module="submodule"></Module>
                </v-expansion-panels>
            </div>
            <div v-if="module.commands != null">
                <h3>Commands</h3>
                <v-expansion-panels>
                    <v-expansion-panel v-for="(command, idx) in module.commands" :key="idx">
                        <v-expansion-panel-header>
                            <span><b>{{command.aliases[0]}}</b> {{command.usage}}</span>
                            <v-spacer />
                            {{command.description}}
                        </v-expansion-panel-header>
                        <v-expansion-panel-content v-if="command.aliases.length > 1">
                            Aliases:
                            <ul>
                                <li v-for="(alias, idx) in command.aliases" :key="idx">{{alias}} {{command.usage}}</li>
                            </ul>
                        </v-expansion-panel-content>
                        <v-expansion-panel-content v-else>
                            This command has no aliases!
                        </v-expansion-panel-content>
                    </v-expansion-panel>
                </v-expansion-panels>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>
</template>

<script>
  export default {
    name: 'Module',
    props: ['module'],
    data: () => ({

    })
  }
</script>
