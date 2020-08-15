import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

  const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/help',
    name: 'Help',
    // route level code-splitting
    component: () => import(/* webpackChunkName: "help" */ '../views/Help.vue')
  }
]

const router = new VueRouter({
  routes
})

export default router
