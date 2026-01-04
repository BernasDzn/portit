<script setup lang="ts">
import { RouterLink } from 'vue-router';
import {useI18n} from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{
  route: string;
}>();



</script>

<template>
  <div class="port3d-hero" aria-label="3D Port Visualization Card">
    <div class="hero-bg"></div>
    <div class="hero-particles">
      <div class="particle" v-for="n in 6" :key="n" :style="`--delay: ${n * 0.5}s; --x: ${Math.random() * 100}%`"></div>
    </div>
    
    <div class="hero-content" role="region">
      <div class="hero-left">
        <div class="hero-badge">
          <span class="badge-dot"></span>
          <span>3D Experience</span>
        </div>
        <h3 class="hero-title">
          {{ t('dashboard.3DModel.title') }}
        </h3>
        <p class="hero-description">
          {{ t('dashboard.3DModel.description') }}
        </p>
        <div class="hero-actions">
          <RouterLink :to="props.route" class="hero-cta-link">
            <button class="hero-cta">
              <span class="cta-content">
                <span class="material-symbols-outlined">open_in_new</span>
                {{ t('dashboard.3DModel.open') }}
              </span>
              <span class="cta-glow"></span>
            </button>
          </RouterLink>
        </div>
      </div>

      <div class="hero-right" aria-hidden="true">
        <div class="hero-icon-container">
          <div class="icon-orbit">
            <div class="orbit-ring ring-1"></div>
            <div class="orbit-ring ring-2"></div>
            <div class="orbit-ring ring-3"></div>
          </div>
          <div class="icon-core">
            <span class="material-symbols-outlined cube">deployed_code</span>
          </div>
          <div class="icon-particles">
            <div class="mini-particle" v-for="n in 4" :key="n" :style="`--angle: ${n * 90}deg`"></div>
          </div>
        </div>
      </div>
    </div>

    <div class="hero-border-glow"></div>
  </div>
</template>

<style scoped>
.port3d-hero {
  position: relative;
  width: 100%;
  margin-top: 2rem;
  border-radius: 28px;
  overflow: hidden;
  isolation: isolate;
  animation: heroSlideIn 0.8s ease-out backwards;
  animation-delay: 0.6s;
}

@keyframes heroSlideIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

.hero-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, 
    #1e1b4b 0%, 
    #312e81 25%, 
    #4338ca 50%, 
    #6366f1 75%,
    #818cf8 100%);
  z-index: -3;
}

.hero-bg::before {
  content: '';
  position: absolute;
  inset: 0;
  background: 
    radial-gradient(ellipse at 20% 80%, rgba(139, 92, 246, 0.4) 0%, transparent 50%),
    radial-gradient(ellipse at 80% 20%, rgba(6, 182, 212, 0.3) 0%, transparent 50%),
    radial-gradient(ellipse at 50% 50%, rgba(236, 72, 153, 0.2) 0%, transparent 60%);
  animation: bgPulse 8s ease-in-out infinite;
}

@keyframes bgPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.hero-particles {
  position: absolute;
  inset: 0;
  overflow: hidden;
  z-index: -1;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  left: var(--x);
  animation: particleFloat 6s ease-in-out infinite;
  animation-delay: var(--delay);
}

@keyframes particleFloat {
  0%, 100% { 
    transform: translateY(100vh) scale(0);
    opacity: 0;
  }
  10% { opacity: 1; transform: translateY(80vh) scale(1); }
  90% { opacity: 1; transform: translateY(-10vh) scale(1); }
  100% { 
    transform: translateY(-20vh) scale(0);
    opacity: 0;
  }
}

.hero-content {
  display: flex;
  gap: 3rem;
  align-items: center;
  justify-content: space-between;
  padding: 2.5rem 3rem;
  position: relative;
  z-index: 1;
}

.hero-left {
  flex: 1 1 60%;
  min-width: 0;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  padding: 0.4rem 1rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 1rem;
}

.badge-dot {
  width: 6px;
  height: 6px;
  background: #22c55e;
  border-radius: 50%;
  animation: dotPulse 2s ease-in-out infinite;
}

@keyframes dotPulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.3); }
}

.hero-title {
  margin: 0 0 0.75rem 0;
  font-size: 2rem;
  font-weight: 700;
  color: #fff;
  letter-spacing: -0.02em;
  text-shadow: 0 2px 20px rgba(0, 0, 0, 0.3);
}

.hero-description {
  margin: 0 0 1.5rem 0;
  color: rgba(255, 255, 255, 0.85);
  max-width: 50ch;
  line-height: 1.6;
  font-size: 1rem;
}

.hero-actions {
  display: flex;
  gap: 1rem;
}

.hero-cta-link {
  text-decoration: none;
}

.hero-cta {
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.875rem 1.75rem;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  border-radius: 14px;
  font-size: 0.95rem;
  font-weight: 600;
  color: #4338ca;
  cursor: pointer;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 
    0 4px 15px rgba(0, 0, 0, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.5);
}

.cta-content {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  position: relative;
  z-index: 1;
}

.cta-content .material-symbols-outlined {
  font-size: 20px;
  transition: transform 0.3s ease;
}

.hero-cta:hover .cta-content .material-symbols-outlined {
  transform: translateX(2px) translateY(-2px);
}

.cta-glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.hero-cta:hover {
  transform: translateY(-3px);
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.3),
    0 0 40px rgba(99, 102, 241, 0.4);
}

.hero-cta:hover .cta-glow {
  opacity: 0.1;
}

.hero-right {
  flex: 0 0 220px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hero-icon-container {
  position: relative;
  width: 180px;
  height: 180px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-orbit {
  position: absolute;
  inset: 0;
}

.orbit-ring {
  position: absolute;
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.15);
}

.ring-1 {
  inset: 0;
  animation: orbitRotate 20s linear infinite;
}

.ring-2 {
  inset: 15%;
  animation: orbitRotate 15s linear infinite reverse;
  border-style: dashed;
}

.ring-3 {
  inset: 30%;
  animation: orbitRotate 10s linear infinite;
  border-width: 2px;
  border-color: rgba(255, 255, 255, 0.25);
}

@keyframes orbitRotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.icon-core {
  position: relative;
  width: 90px;
  height: 90px;
  border-radius: 24px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0.05) 100%);
  backdrop-filter: blur(10px);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 
    0 8px 32px rgba(0, 0, 0, 0.2),
    inset 0 1px 1px rgba(255, 255, 255, 0.3);
  animation: coreFloat 4s ease-in-out infinite;
}

@keyframes coreFloat {
  0%, 100% { transform: translateY(0) rotate(0deg); }
  50% { transform: translateY(-8px) rotate(5deg); }
}

.cube {
  font-size: 48px;
  color: #ffffff;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3));
}

.icon-particles {
  position: absolute;
  inset: 0;
}

.mini-particle {
  position: absolute;
  width: 8px;
  height: 8px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 50%;
  top: 50%;
  left: 50%;
  transform: rotate(var(--angle)) translateX(80px) translateY(-50%);
  animation: particlePulse 3s ease-in-out infinite;
  animation-delay: calc(var(--angle) / 360 * 3s);
}

@keyframes particlePulse {
  0%, 100% { opacity: 0.3; transform: rotate(var(--angle)) translateX(80px) translateY(-50%) scale(1); }
  50% { opacity: 1; transform: rotate(var(--angle)) translateX(80px) translateY(-50%) scale(1.5); }
}

.hero-border-glow {
  position: absolute;
  inset: 0;
  border-radius: 28px;
  padding: 2px;
  background: linear-gradient(135deg, 
    rgba(255, 255, 255, 0.4),
    rgba(139, 92, 246, 0.3),
    rgba(6, 182, 212, 0.3),
    rgba(255, 255, 255, 0.2));
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  pointer-events: none;
  z-index: 2;
}

/* Responsive */
@media (max-width: 768px) {
  .hero-content {
    flex-direction: column;
    padding: 2rem 1.5rem;
    text-align: center;
  }
  
  .hero-left {
    order: 2;
  }
  
  .hero-right {
    order: 1;
    margin-bottom: 1rem;
  }
  
  .hero-badge {
    margin: 0 auto 1rem;
  }
  
  .hero-title {
    font-size: 1.5rem;
  }
  
  .hero-description {
    max-width: 100%;
  }
  
  .hero-actions {
    justify-content: center;
  }
  
  .hero-icon-container {
    width: 140px;
    height: 140px;
  }
  
  .icon-core {
    width: 70px;
    height: 70px;
  }
  
  .cube {
    font-size: 36px;
  }
}
</style>
