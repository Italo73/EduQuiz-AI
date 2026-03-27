<template>
  <div class="app-layout">

    <aside class="sidebar">
      <div class="logo">
        <div class="logo-icon">E</div>
        <div>
          <div class="logo-name">EduQuiz</div>
          <div class="logo-sub">AI</div>
        </div>
      </div>

      <nav class="sidebar-nav">
        <button class="nav-item" :class="{ active: vistaActiva === 'lista' }" @click="vistaActiva = 'lista'">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/>
            <rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/>
          </svg>
          Mis evaluaciones
          <span v-if="evaluaciones.length" class="count-badge">{{ evaluaciones.length }}</span>
        </button>

        <button class="nav-item" :class="{ active: vistaActiva === 'nueva' }" @click="vistaActiva = 'nueva'">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
          </svg>
          Nueva con IA
        </button>
      </nav>

      <div class="sidebar-footer">
        <div class="ai-badge">
          <span class="ai-dot"></span>
          Powered by Claude AI
        </div>
      </div>
    </aside>

    <main class="main-content">

      <div v-if="vistaActiva === 'nueva'" class="view">
        <div class="view-header">
          <h1>Crear evaluación</h1>
          <p>La IA generará preguntas de selección múltiple adaptadas al nivel y tema.</p>
        </div>
        <GeneradorForm @creada="onEvaluacionCreada" />
      </div>

      <div v-else class="view">
        <div class="view-header">
          <h1>Mis evaluaciones</h1>
          <p>{{ evaluaciones.length === 0
            ? 'Aún no hay evaluaciones. ¡Crea la primera!'
            : `${evaluaciones.length} evaluación${evaluaciones.length !== 1 ? 'es' : ''} guardada${evaluaciones.length !== 1 ? 's' : ''}` }}</p>
        </div>

        <div v-if="loadingLista" class="empty-state">
          <div class="loading-ring"></div>
          <p>Cargando evaluaciones...</p>
        </div>

        <div v-else-if="evaluaciones.length === 0" class="empty-state">
          <div class="empty-icon">✦</div>
          <h3>Aún no hay evaluaciones</h3>
          <p>Genera tu primera evaluación con IA</p>
          <button class="btn-cta" @click="vistaActiva = 'nueva'">Crear ahora</button>
        </div>

        <div v-else class="evaluaciones-grid">
          <EvaluacionCard
            v-for="ev in evaluaciones"
            :key="ev.id"
            :evaluacion="ev"
            @eliminar="eliminar"
          />
        </div>
      </div>

    </main>

    <Transition name="toast">
      <div v-if="toast.visible" class="toast" :class="`toast--${toast.type}`">
        {{ toast.msg }}
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import GeneradorForm from './components/GeneradorForm.vue'
import EvaluacionCard from './components/EvaluacionCard.vue'
import { api } from './api/evaluaciones.js'

const vistaActiva = ref('lista')
const evaluaciones = ref([])
const loadingLista = ref(true)
const toast = ref({ visible: false, msg: '', type: 'success' })

function mostrarToast(msg, type = 'success') {
  toast.value = { visible: true, msg, type }
  setTimeout(() => { toast.value.visible = false }, 3000)
}

async function cargarEvaluaciones() {
  loadingLista.value = true
  try {
    evaluaciones.value = await api.getAll()
  } catch {
    mostrarToast('Error cargando evaluaciones', 'error')
  } finally {
    loadingLista.value = false
  }
}

async function eliminar(id) {
  try {
    await api.delete(id)
    evaluaciones.value = evaluaciones.value.filter(e => e.id !== id)
    mostrarToast('Evaluación eliminada')
  } catch {
    mostrarToast('Error al eliminar', 'error')
  }
}

function onEvaluacionCreada(evaluacion) {
  evaluaciones.value.unshift(evaluacion)
  vistaActiva.value = 'lista'
  mostrarToast(`✓ Evaluación "${evaluacion.tema}" creada con ${evaluacion.preguntas.length} preguntas`)
}

onMounted(cargarEvaluaciones)
</script>

<style scoped>
.app-layout { display: flex; min-height: 100vh; }

.sidebar {
  width: 240px;
  background: var(--ink);
  display: flex;
  flex-direction: column;
  padding: 1.75rem 1rem;
  flex-shrink: 0;
  position: sticky;
  top: 0;
  height: 100vh;
}
.logo { display: flex; align-items: center; gap: 0.75rem; padding: 0 0.5rem; margin-bottom: 2.5rem; }
.logo-icon {
  width: 38px; height: 38px;
  background: var(--accent);
  border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-family: 'Fraunces', serif;
  font-size: 1.3rem; color: white; font-weight: 700; flex-shrink: 0;
}
.logo-name { font-family: 'Fraunces', serif; font-size: 1rem; color: white; font-weight: 600; line-height: 1; }
.logo-sub { font-size: 0.65rem; color: var(--accent); letter-spacing: 0.15em; text-transform: uppercase; font-weight: 500; }
.sidebar-nav { display: flex; flex-direction: column; gap: 0.25rem; flex: 1; }
.nav-item {
  display: flex; align-items: center; gap: 0.75rem;
  padding: 0.7rem 0.85rem;
  border-radius: var(--radius);
  font-size: 0.9rem;
  color: rgba(255,255,255,0.55);
  background: none; border: none;
  text-align: left; cursor: pointer;
  transition: all 0.18s;
}
.nav-item:hover { background: rgba(255,255,255,0.07); color: rgba(255,255,255,0.85); }
.nav-item.active { background: rgba(212,98,42,0.2); color: #f0a080; }
.count-badge {
  margin-left: auto;
  background: rgba(255,255,255,0.12);
  color: rgba(255,255,255,0.6);
  font-size: 0.72rem;
  padding: 0.1rem 0.5rem;
  border-radius: 10px;
}
.sidebar-footer { padding-top: 1rem; }
.ai-badge { display: flex; align-items: center; gap: 0.5rem; font-size: 0.75rem; color: rgba(255,255,255,0.3); padding: 0 0.5rem; }
.ai-dot {
  width: 6px; height: 6px;
  border-radius: 50%;
  background: #4ade80;
  box-shadow: 0 0 6px #4ade80;
  animation: pulse 2s infinite;
}
@keyframes pulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.5; } }

.main-content { flex: 1; padding: 2.5rem 2.5rem 3rem; max-width: 860px; }
.view-header { margin-bottom: 2rem; }
.view-header h1 { font-size: 2rem; color: var(--ink); margin-bottom: 0.35rem; }
.view-header p { color: var(--ink-muted); font-size: 0.95rem; }

.empty-state { text-align: center; padding: 4rem 2rem; color: var(--ink-muted); }
.empty-icon { font-size: 2.5rem; color: var(--border); margin-bottom: 1rem; }
.empty-state h3 { font-family: 'Fraunces', serif; color: var(--ink-soft); margin-bottom: 0.4rem; }
.empty-state p { margin-bottom: 1.5rem; }
.btn-cta {
  background: var(--accent); color: white; border: none;
  padding: 0.7rem 1.5rem; border-radius: var(--radius);
  font-size: 0.95rem; font-weight: 500; cursor: pointer;
  transition: background 0.2s;
}
.btn-cta:hover { background: var(--accent-dark); }
.loading-ring {
  width: 36px; height: 36px;
  border: 3px solid var(--border);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 1rem;
}
@keyframes spin { to { transform: rotate(360deg); } }

.evaluaciones-grid { display: flex; flex-direction: column; gap: 0.85rem; }

.toast {
  position: fixed; bottom: 2rem; right: 2rem;
  padding: 0.85rem 1.25rem;
  border-radius: var(--radius);
  font-size: 0.9rem; font-weight: 500;
  z-index: 999;
  box-shadow: var(--shadow-lg);
}
.toast--success { background: var(--success-light); color: var(--success); border: 1px solid #b2dfcc; }
.toast--error { background: #fff0ee; color: var(--accent-dark); border: 1px solid #fca99a; }
.toast-enter-active, .toast-leave-active { transition: all 0.3s ease; }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(10px); }

@media (max-width: 700px) {
  .app-layout { flex-direction: column; }
  .sidebar { width: 100%; height: auto; flex-direction: row; flex-wrap: wrap; padding: 1rem; }
  .logo { margin-bottom: 0; }
  .sidebar-nav { flex-direction: row; }
  .sidebar-footer { display: none; }
  .main-content { padding: 1.5rem; max-width: 100%; }
}
</style>