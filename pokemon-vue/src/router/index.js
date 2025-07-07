import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import Login from '@/views/Login.vue';
import PokemonSearch from '@/views/PokemonSearch.vue';
import Signup from '../views/Signup.vue';

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login }, 
  { path: '/sign-up', component: Signup },
  { path: '/search', component: PokemonSearch, meta: { requiresAuth: true } },
  { path: '/:pathMatch(.*)*', redirect: '/login' }
];


const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const auth = useAuthStore();
  if (to.meta.requiresAuth && !auth.isLoggedIn) return next('/login');
  next();
});
export default router;
