import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        auth: {},
        botInfo: {}
    },
    mutations: {
        login(state, auth) {
            state.auth = auth
        },
        botInfo(state, botInfo) {
            state.botInfo = botInfo
        }
    },
    actions: {
        login({ commit }) {
            axios.get("/api/auth/whoami")
                .then(response => commit('login', response.data))
        },
        botInfo({ commit }) {
            axios.get("/api/botInfo")
                .then(response => commit('botInfo', response.data))
        }
    },
    modules: {
    }
})
