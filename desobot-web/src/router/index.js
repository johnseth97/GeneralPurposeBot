import Vue from 'vue'
import VueRouter from 'vue-router'

import Home from '@/views/Home.vue'
import Help from '@/views/Help.vue'

Vue.use(VueRouter)

  const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/help/:serverId',
    name: 'Help',
    // route level code-splitting
    //component: () => import(/* webpackChunkName: "help" */ '../views/Help.vue')
    component: Help,
    props: true
  }
]

const router = new VueRouter({
  routes
})

export default router
