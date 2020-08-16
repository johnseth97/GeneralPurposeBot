<template>
    <v-content>
        <v-container>
            <v-card class="pd-8" v-if="$store.state.auth.authenticated">
                <v-row align="center">
                    <v-spacer />
                    <v-col cols="10">
                        <v-select :items="servers"
                                  label="Server"
                                  item-text="name"
                                  item-value="id"
                                  v-model="selectedServerId"
                                  @change="updateFromDropdown">
                        </v-select>
                    </v-col>
                    <v-spacer />
                </v-row>
            </v-card>
        </v-container>
        <v-container>
            <v-expansion-panels>
                <Module v-for="(module, idx) in modules" :key="idx" :module="module"></Module>
            </v-expansion-panels>
        </v-container>
    </v-content>
</template>

<script>
    import axios from 'axios';
    import Module from '@/components/Module.vue'
    import store from '@/store/index.js'
    import router from '@/router/index.js'

    export default {
        name: 'Help',
        data: () => ({
            modules: [], 
            servers: [],
            selectedServerId: 0
        }),
        props: ['serverId'],
        mounted() {
            this.selectedServerId = this.serverId
            this.updateModuleList();
            this.servers.push({ name: "Global", id: 0 });
            for (var server of store.state.auth.guilds) {
                this.servers.push({ name: server.name, id: server.id });
            }
        },
        components: {
            Module
        },
        methods: {
            updateModuleList() {
                if (this.serverId == 0) {
                    console.log("getting globally enabled modules")
                    axios
                        .get("/api/help")
                        .then(response => (this.modules = response.data))
                    return;
                } else {
                    console.log("getting modules for server " + this.serverId)
                    axios
                        .get("/api/help/" + this.serverId)
                        .then(response => (this.modules = response.data))       
                }
            },
            updateFromDropdown() {
                router.push({ name: 'Help', params: { serverId: this.selectedServerId } })
            }
        },
        watch: {
            $route(to) {
                console.log(to);
                this.serverId = to.params.serverId
                this.selectedServerId = to.params.serverId
                this.updateModuleList();
            }
        }
    }
</script>