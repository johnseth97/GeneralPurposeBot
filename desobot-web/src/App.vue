<template>
    <v-app id="desobot">
        <v-navigation-drawer v-model="drawer"
                             app
                             clipped>
            <v-list dense>
                <v-list-item link to="/">
                    <v-list-item-action>
                        <v-icon>mdi-home</v-icon>
                    </v-list-item-action>
                    <v-list-item-content>
                        <v-list-item-title>Home</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item link to="/help">
                    <v-list-item-action>
                        <v-icon>mdi-help-circle</v-icon>
                    </v-list-item-action>
                    <v-list-item-content>
                        <v-list-item-title>Help</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
            </v-list>
        </v-navigation-drawer>

        <v-app-bar app
                   clipped-left>
            <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
            <v-toolbar-title>DesoBot</v-toolbar-title>
            <v-spacer/>
            <v-btn v-if="!$store.state.auth.authenticated" href="/api/auth/signin" color="primary">Log in with Discord</v-btn>
            <v-menu v-else :offset-y="true">
                <template v-slot:activator="{ on, attrs }">
                    <v-btn text dark v-bind="attrs" v-on="on">
                        Logged in as
                        {{$store.state.auth.username}}#{{$store.state.auth.discriminator}}
                        <v-icon>mdi-menu-down</v-icon>
                    </v-btn>
                </template>
                <v-card>
                    <v-img class="white--text align-end"
                           :src="$store.state.auth.avatarUrl">
                        <v-card-title>{{$store.state.auth.username}}#{{$store.state.auth.discriminator}}</v-card-title>
                    </v-img>
                    <v-card-actions>
                        <v-spacer/>
                        <v-btn color="red" text href="/api/auth/signout">Log Out</v-btn>
                    </v-card-actions>
                </v-card>
            </v-menu>
        </v-app-bar>

        <router-view></router-view>
        <v-footer app>
            <span>&copy; Mitchell Monahan/Ethan Johnson {{ new Date().getFullYear() }}. <a href="https://github.com/EthanJohnson97/GeneralPurposeBot">Contribute on GitHub!</a></span>
        </v-footer>
    </v-app>
</template>

<script>
    import store from '@/store/index.js'
    export default {
        props: {

        },

        data: () => ({
            drawer: false,
        }),

        created() {
            this.$vuetify.theme.dark = true
            store.dispatch('login')
        }
    }
</script>
