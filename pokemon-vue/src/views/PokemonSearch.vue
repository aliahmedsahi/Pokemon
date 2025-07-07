<template>
  <div class="spp-auth-wrapper">
    <div class="spp-auth-container">
      <div class="spp-topbar">
      <Button 
        label="Logout" 
        icon="pi pi-sign-out" 
        class="p-button-text p-button-danger" 
        @click="logout" 
      />
    </div>
      <div class="spp-search-panel">
        <h1>Pokémon Search</h1>
        <div class="spp-search-input-group">
          <InputText v-model="query" placeholder="Enter Pokémon name or ID" class="spp-input-text" />
          <Button label="Search" icon="pi pi-search" class="spp-search-button" @click="search" />
          <Button label="Show All" icon="pi pi-list" class="spp-show-all-button" @click="showAll" />
        </div>
        <ProgressSpinner v-if="loading" style="width: 50px; height: 50px;" strokeWidth="4" class="my-4" />
        <p v-if="errorMessage" class="text-warning">{{ errorMessage }}</p>

        <!-- Single result -->
        <div v-if="pokemon" class="spp-pokemon-result spp-d-flex-row">
          <div class="spp-details-section">
            <div class="spp-pokemon-name">{{ pokemon.name }}</div>
            <div class="pokemon-detail">
              <div class="detail-row">
                <span class="label">Abilities:</span>
                <span class="value">{{ pokemon.abilities.join(', ') }}</span>
              </div>
              <div class="detail-row">
                <span class="label">Height:</span>
                <span class="value">{{ pokemon.height }}</span>
              </div>
              <div class="detail-row">
                <span class="label">Weight:</span>
                <span class="value">{{ pokemon.weight }}</span>
              </div>
              <div class="detail-row">
                <span class="label">Types:</span>
                <span class="value type-container">
                  <span v-for="type in pokemon.types" :key="type" class="type-badge"
                    :style="{ backgroundColor: getTypeColor(type) }">
                    {{ type }}
                  </span>
                </span>
              </div>
            </div>
          </div>

          <div class="spp-image-section">
            <img :src="pokemon.sprite" alt="pokemon sprite" class="spp-pokemon-image" />
          </div>
        </div>

        <!-- All from DB -->
        <div v-if="showAllList" class="spp-all-pokemon-grid mt-4">
          <div v-for="poke in allPokemon" :key="poke.id" class="spp-pokemon-card spp-d-flex-row">

            <div class="spp-details-section">
              <div class="spp-pokemon-name">{{ poke.name }}</div>
              <div class="pokemon-detail">
                <div class="detail-row">
                  <span class="label">Abilities:</span>
                  <span class="value">{{ poke.abilities.join(', ') }}</span>
                </div>
                <hr class="detail-separator" />

                <div class="detail-row">
                  <span class="label">Height:</span>
                  <span class="value">{{ poke.height }}</span>
                </div>
                <hr class="detail-separator" />

                <div class="detail-row">
                  <span class="label">Weight:</span>
                  <span class="value">{{ poke.weight }}</span>
                </div>
                <hr class="detail-separator" />

                <div class="detail-row">
                  <span class="label">Types:</span>
                  <span class="value type-container">
                    <span v-for="type in poke.types" :key="type" class="type-badge"
                      :style="{ backgroundColor: getTypeColor(type) }">
                      {{ type }}
                    </span>
                  </span>
                </div>
                <hr class="detail-separator" />
              </div>
            </div>

            <div class="spp-image-section">
              <img :src="poke.sprite" :alt="poke.name" class="spp-pokemon-image" />
            </div>

          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup>
import { ref } from 'vue';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import api from '@/services/api';
import ProgressSpinner from 'primevue/progressspinner';
import { useRouter } from 'vue-router';

const query = ref('');
const pokemon = ref(null);
const allPokemon = ref([]);
const errorMessage = ref('');
const showAllList = ref(false);
const loading = ref(false);

const router = useRouter();

async function search() {
  errorMessage.value = '';
  pokemon.value = null;
  showAllList.value = false; 
  loading.value = true;

  if (!query.value.trim()) {
    errorMessage.value = 'Please enter a Pokémon name or ID.';
    loading.value = false;
    return;
  }

  try {
    const res = await api.get(`/pokemon/${query.value.toLowerCase()}`);
    pokemon.value = res.data;
  } catch {
    errorMessage.value = 'Pokémon not found. Please try another name or ID.';
  } finally {
    loading.value = false;
  }
}

async function showAll() {
  errorMessage.value = '';
  pokemon.value = null;
  showAllList.value = true;

  if (allPokemon.value.length === 0) {
    await fetchAllPokemonFromDb();
  }
}

function logout() {
  localStorage.removeItem('auth_token');
  router.push('/login');
}

async function fetchAllPokemonFromDb() {
  try {
    const res = await api.get('/pokemon/get-all-pokemon');
    allPokemon.value = res.data;
  } catch {
    errorMessage.value = 'Could not load Pokémon from database.';
  } finally {
    loading.value = false;
  } 

}

function getTypeColor(type) {
  const colors = {
    normal: '#A8A77A', fire: '#EE8130', water: '#6390F0', electric: '#F7D02C',
    grass: '#7AC74C', ice: '#96D9D6', fighting: '#C22E28', poison: '#A33EA1',
    ground: '#E2BF65', flying: '#A98FF3', psychic: '#F95587', bug: '#A6B91A',
    rock: '#B6A136', ghost: '#735797', dragon: '#6F35FC', dark: '#705746',
    steel: '#B7B7CE', fairy: '#D685AD'
  };
  return colors[type.toLowerCase()] || '#666';
}

</script>
