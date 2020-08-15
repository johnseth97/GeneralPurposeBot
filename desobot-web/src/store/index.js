import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        auth: {}
    },
    mutations: {
        login(state, auth) {
            state.auth = auth
            console.log(auth);
        }
    },
    actions: {
        login({ commit }) {
            axios.get("/api/auth/whoami")
                .then(response => commit('login', response.data))
        }
    },
    modules: {
    }
})
