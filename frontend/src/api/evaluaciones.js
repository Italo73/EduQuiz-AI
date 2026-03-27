const BASE = 'http://localhost:5000/api/evaluaciones'

export const api = {
  async getAll() {
    const res = await fetch(BASE)
    if (!res.ok) throw new Error('Error cargando evaluaciones')
    return res.json()
  },

  async generar(payload) {
    const res = await fetch(`${BASE}/generar`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    const data = await res.json()
    if (!res.ok) throw new Error(data.error || 'Error generando evaluación')
    return data
  },

  async delete(id) {
    const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' })
    if (!res.ok) throw new Error('Error eliminando evaluación')
  }
}