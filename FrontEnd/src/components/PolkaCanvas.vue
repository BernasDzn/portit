<template>
  <canvas ref="canvas" class="polka-canvas" />
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref } from 'vue'

/**
 * Props (tweak as needed)
 */
const props = withDefaults(defineProps<{
  rows?: number
  cols?: number
  gap?: number
  baseSize?: number
  waveSpeed?: number
  waveAmplitude?: number
  waveLength?: number
}>(), {
  rows: 39,
  cols: 50,
  gap: 25,
  baseSize: 1,
  waveSpeed: 0.02,
  waveAmplitude: 0.5,
  waveLength: 20.0,
})

const canvas = ref<HTMLCanvasElement | null>(null)
let ctx: CanvasRenderingContext2D | null = null
let raf = 0
let time = 0
let lastT = 0

const dpr = typeof window !== 'undefined' ? window.devicePixelRatio || 1 : 1

let positions: { x: number; y: number; diagonal: number }[] = []

function computePositions(width: number, height: number) {
  positions = []
  let colsToFill = Math.ceil(width / props.gap)
  let rowsToFill = Math.ceil(height / props.gap)
  // if dots needed to fill more than threshold, skip rendering for performance
  const RENDER_THRESHOLD = 10000
  if (colsToFill * rowsToFill > RENDER_THRESHOLD) {
    positions = []
    return
  }
  const startX = props.gap / 2
  const startY = props.gap / 2

  for (let r = 0; r < rowsToFill; r++) {
    for (let c = 0; c < colsToFill; c++) {
      positions.push({
        x: startX + c * props.gap,
        y: startY + r * props.gap,
        diagonal: c + (rowsToFill - r)
      })
    }
  }
}

function resizeCanvas() {
  if (!canvas.value) return

  const parent = canvas.value.parentElement
  const rect = parent ? parent.getBoundingClientRect() : { width: window.innerWidth, height: window.innerHeight }
  const width = Math.max(rect.width, 1)
  const height = Math.max(rect.height, 1)

  canvas.value.style.width = `${width}px`
  canvas.value.style.height = `${height}px`

  canvas.value.width = Math.round(width * dpr)
  canvas.value.height = Math.round(height * dpr)

  if (!ctx) ctx = canvas.value.getContext('2d')

  if (ctx) {
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0)
    computePositions(width, height)
  }
}

function drawFrame(t: number) {
  if (!ctx || !canvas.value) return
  const width = canvas.value.width / dpr
  const height = canvas.value.height / dpr

  // calculate time delta normalized to 1 update at 60fps
  const delta = (t - lastT) / 16.6667 || 0
  lastT = t
  time = (time + delta) % (Math.PI * 2 / props.waveSpeed)

  ctx.clearRect(0, 0, width, height)

  // draw background gradient
  const g = ctx.createLinearGradient(0, 0, 0, height)
  g.addColorStop(0, '#393357')
  g.addColorStop(1, '#00062d')
  ctx.fillStyle = g
  ctx.fillRect(0, 0, width, height)

  // render dots
  for (let i = 0; i < positions.length; i++) {
    const p = positions[i]!
  const phase = p.diagonal / props.waveLength - time * props.waveSpeed
  const scale = 2 + Math.sin(phase) * props.waveAmplitude
  const size = props.baseSize * scale
  // vertical shift
  const verticalShift = Math.sin(phase) * (props.gap * 0.25)
  const drawY = p.y - verticalShift

    ctx.fillStyle = `rgba(200, 200, 255, ${0.3 + 0.7 * (scale - 1) / (2 + props.waveAmplitude)})`

    ctx.beginPath()
    ctx.arc(p.x, drawY, size, 0, Math.PI * 2)
    ctx.fill()
  }

  raf = requestAnimationFrame(drawFrame)
}

function start() {
  stop()
  lastT = performance.now()
  raf = requestAnimationFrame(drawFrame)
}

function stop() {
  if (raf) cancelAnimationFrame(raf)
  raf = 0
}

onMounted(() => {
  if (!canvas.value) return
  ctx = canvas.value.getContext('2d')
  resizeCanvas()
  window.addEventListener('resize', resizeCanvas)
  start()
})

onBeforeUnmount(() => {
  stop()
  window.removeEventListener('resize', resizeCanvas)
})
</script>

<style scoped>
.polka-canvas {
  display: block;
  width: 100%;
  height: 100%;
  pointer-events: none;
}
</style>