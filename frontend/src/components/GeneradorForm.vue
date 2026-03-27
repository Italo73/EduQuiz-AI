<template>
  <div class="generator-card">
    <div class="card-header">
      <span class="chip">✦ Nueva evaluación</span>
      <h2>Generar con IA</h2>
      <p>Describe el tema y la IA creará preguntas de selección múltiple listas para usar.</p>
    </div>

    <div class="form-body">
      <div class="field">
        <label>Tema</label>
        <input
          v-model="form.tema"
          type="text"
          placeholder="Ej: La fotosíntesis, Revolución Francesa, Fracciones..."
          :disabled="loading"
          @keyup.enter="submit"
        />
      </div>

      <div class="row-fields">
        <div class="field">
          <label>Asignatura</label>
          <select v-model="form.asignatura" :disabled="loading">
            <option v-for="a in asignaturas" :key="a" :value="a">{{ a }}</option>
          </select>
        </div>

        <div class="field">
          <label>Nivel</label>
          <select v-model="form.nivel" :disabled="loading">
            <option v-for="n in niveles" :key="n" :value="n">{{ n }}</option>
          </select>
        </div>

        <div class="field field--small">
          <label>Preguntas</label>
          <select v-model="form.cantidad" :disabled="loading">
            <option :value="3">3</option>
            <option :value="5">5</option>
            <option :value="8">8</option>
            <option :value="10">10</option>
          </select>
        </div>
      </div>

      <div v-if="error" class="error-msg">⚠ {{ error }}</div>

      <button class="btn-generate" @click="submit" :disabled="loading">
        <span v-if="!loading">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
          </svg>
          Generar evaluación
        </span>
        <span v-else class="loading-text">
          <span class="spinner"></span>
          Consultando a la IA...
        </span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { api } from '../api/evaluaciones.js'

const emit = defineEmits(['creada'])

const loading = ref(false)
const error = ref('')

const asignaturas = [
  'Ciencias Naturales', 'Historia', 'Matemáticas', 'Lenguaje',
  'Inglés', 'Física', 'Química', 'Biología', 'Educación Física', 'Artes'
]

const niveles = [
  '1° Básico', '2° Básico', '3° Básico', '4° Básico',
  '5° Básico', '6° Básico', '7° Básico', '8° Básico',
  '1° Medio', '2° Medio', '3° Medio', '4° Medio'
]

const form = reactive({
  tema: '',
  asignatura: 'Ciencias Naturales',
  nivel: '5° Básico',
  cantidad: 5
})

async function submit() {
  error.value = ''
  if (!form.tema.trim()) {
    error.value = 'Por favor ingresa un tema para la evaluación.'
    return
  }
  loading.value = true
  try {
    const evaluacion = await api.generar({ ...form })
    emit('creada', evaluacion)
    form.tema = ''
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.generator-card {
  background: var(--card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  overflow: hidden;
  border: 1px solid var(--border);
}
.card-header {
  background: linear-gradient(135deg, var(--ink) 0%, #2d2d50 100%);
  color: white;
  padding: 2rem 2rem 1.75rem;
}
.chip {
  display: inline-block;
  font-size: 0.72rem;
  font-weight: 500;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  background: rgba(255,255,255,0.15);
  color: rgba(255,255,255,0.85);
  padding: 0.3rem 0.75rem;
  border-radius: 20px;
  margin-bottom: 0.75rem;
}
.card-header h2 { font-size: 1.75rem; font-weight: 600; color: white; margin-bottom: 0.4rem; }
.card-header p { font-size: 0.9rem; color: rgba(255,255,255,0.65); font-weight: 300; }
.form-body { padding: 1.75rem 2rem 2rem; display: flex; flex-direction: column; gap: 1.25rem; }
.field { display: flex; flex-direction: column; gap: 0.4rem; flex: 1; }
.field--small { flex: 0 0 90px; }
.row-fields { display: flex; gap: 0.75rem; flex-wrap: wrap; }
label { font-size: 0.8rem; font-weight: 500; color: var(--ink-soft); letter-spacing: 0.03em; text-transform: uppercase; }
input, select {
  padding: 0.7rem 0.9rem;
  border: 1.5px solid var(--border);
  border-radius: var(--radius);
  font-size: 0.95rem;
  color: var(--ink);
  background: var(--surface);
  transition: border-color 0.2s, box-shadow 0.2s;
  outline: none;
  width: 100%;
}
input:focus, select:focus {
  border-color: var(--accent);
  box-shadow: 0 0 0 3px var(--accent-light);
  background: white;
}
input:disabled, select:disabled { opacity: 0.5; cursor: not-allowed; }
.error-msg {
  background: #fff5f0;
  color: var(--accent-dark);
  border: 1px solid #fcd5c0;
  padding: 0.7rem 1rem;
  border-radius: var(--radius);
  font-size: 0.88rem;
}
.btn-generate {
  background: var(--accent);
  color: white;
  border: none;
  padding: 0.85rem 1.5rem;
  border-radius: var(--radius);
  font-size: 1rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: background 0.2s, transform 0.1s, box-shadow 0.2s;
  box-shadow: 0 4px 14px rgba(212, 98, 42, 0.3);
}
.btn-generate:hover:not(:disabled) {
  background: var(--accent-dark);
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(212, 98, 42, 0.4);
}
.btn-generate:disabled { background: var(--ink-muted); box-shadow: none; cursor: not-allowed; }
.loading-text { display: flex; align-items: center; gap: 0.6rem; }
.spinner {
  width: 16px; height: 16px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  display: inline-block;
}
@keyframes spin { to { transform: rotate(360deg); } }
</style>