<template>
    <v-content>
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
    export default {
        name: 'Help',
        data: () => ({
            modules: []
        }),
        mounted() {
            axios
                .get("/api/help")
                .then(response => (this.modules = response.data))
                .then(response => console.log(response))
        },
        components: {
            Module
        }
    }
</script>