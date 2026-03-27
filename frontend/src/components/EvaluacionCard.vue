<template>
  <div class="ev-card" :class="{ 'is-expanded': expanded }">
    <div class="ev-header" @click="expanded = !expanded">
      <div class="ev-meta">
        <span class="asignatura-badge">{{ evaluacion.asignatura }}</span>
        <h3>{{ evaluacion.tema }}</h3>
        <div class="ev-tags">
          <span class="tag">{{ evaluacion.nivel }}</span>
          <span class="tag">{{ evaluacion.preguntas.length }} preguntas</span>
          <span class="tag tag--date">{{ formatDate(evaluacion.creadoEn) }}</span>
        </div>
      </div>
      <div class="ev-actions">
        <button class="btn-delete" @click.stop="$emit('eliminar', evaluacion.id)" title="Eliminar">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14H6L5 6"/>
            <path d="M10 11v6M14 11v6"/><path d="M9 6V4h6v2"/>
          </svg>
        </button>
        <button class="btn-toggle" :class="{ rotated: expanded }">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="6 9 12 15 18 9"/>
          </svg>
        </button>
      </div>
    </div>

    <div v-if="expanded" class="preguntas-list">
      <div v-for="(p, i) in evaluacion.preguntas" :key="p.id" class="pregunta-item">
        <div class="pregunta-num">{{ i + 1 }}</div>
        <div class="pregunta-content">
          <p class="enunciado">{{ p.enunciado }}</p>
          <div class="opciones">
            <div
              v-for="(opcion, j) in opciones(p)"
              :key="j"
              class="opcion"
              :class="{ 'opcion--correcta': j === p.respuestaCorrectaIndex }"
            >
              <span class="letra">{{ letras[j] }}</span>
              <span>{{ opcion }}</span>
              <span v-if="j === p.respuestaCorrectaIndex" class="correcta-icon">✓</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const props = defineProps({ evaluacion: { type: Object, required: true } })
defineEmits(['eliminar'])

const expanded = ref(false)
const letras = ['A', 'B', 'C', 'D']

function opciones(p) {
  return [p.opcionA, p.opcionB, p.opcionC, p.opcionD]
}

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('es-CL', {
    day: '2-digit', month: 'short', year: 'numeric'
  })
}
</script>

<style scoped>
.ev-card {
  background: var(--card);
  border: 1.5px solid var(--border);
  border-radius: var(--radius-lg);
  overflow: hidden;
  transition: box-shadow 0.2s, border-color 0.2s;
}
.ev-card:hover, .ev-card.is-expanded {
  box-shadow: var(--shadow-lg);
  border-color: transparent;
}
.ev-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  cursor: pointer;
  user-select: none;
  gap: 1rem;
}
.ev-meta { flex: 1; min-width: 0; }
.asignatura-badge {
  display: inline-block;
  font-size: 0.72rem;
  font-weight: 500;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--teal);
  background: var(--teal-light);
  padding: 0.2rem 0.6rem;
  border-radius: 20px;
  margin-bottom: 0.4rem;
}
h3 {
  font-family: 'Fraunces', serif;
  font-size: 1.1rem;
  color: var(--ink);
  font-weight: 600;
  margin-bottom: 0.5rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.ev-tags { display: flex; gap: 0.4rem; flex-wrap: wrap; }
.tag { font-size: 0.78rem; color: var(--ink-soft); background: var(--surface-alt); padding: 0.15rem 0.55rem; border-radius: 8px; }
.tag--date { color: var(--ink-muted); }
.ev-actions { display: flex; gap: 0.5rem; align-items: center; flex-shrink: 0; }
.btn-delete {
  background: none;
  border: 1.5px solid var(--border);
  color: var(--ink-muted);
  width: 34px; height: 34px;
  border-radius: 8px;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.btn-delete:hover { background: #fff0ee; border-color: #fca99a; color: #c0392b; }
.btn-toggle {
  background: none; border: none;
  color: var(--ink-muted);
  width: 34px; height: 34px;
  display: flex; align-items: center; justify-content: center;
  transition: transform 0.25s;
}
.btn-toggle.rotated { transform: rotate(180deg); }
.preguntas-list {
  border-top: 1.5px solid var(--surface-alt);
  padding: 1rem 1.5rem 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  animation: slideDown 0.2s ease;
}
@keyframes slideDown {
  from { opacity: 0; transform: translateY(-8px); }
  to   { opacity: 1; transform: translateY(0); }
}
.pregunta-item { display: flex; gap: 1rem; }
.pregunta-num {
  font-family: 'Fraunces', serif;
  font-size: 1.2rem;
  font-weight: 600;
  color: var(--accent);
  min-width: 28px;
  padding-top: 0.1rem;
}
.pregunta-content { flex: 1; }
.enunciado { font-size: 0.95rem; font-weight: 500; color: var(--ink); margin-bottom: 0.75rem; line-height: 1.5; }
.opciones { display: flex; flex-direction: column; gap: 0.4rem; }
.opcion {
  display: flex; align-items: center; gap: 0.6rem;
  padding: 0.45rem 0.75rem;
  border-radius: 8px;
  font-size: 0.88rem;
  color: var(--ink-soft);
  background: var(--surface);
}
.opcion--correcta { background: var(--success-light); color: var(--success); font-weight: 500; }
.letra { font-weight: 600; min-width: 18px; font-size: 0.82rem; }
.correcta-icon { margin-left: auto; font-size: 0.85rem; font-weight: 700; }
</style>