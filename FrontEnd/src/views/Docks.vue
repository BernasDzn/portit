<script setup lang="ts">
	import { ref, onMounted } from 'vue'
	import Loading from '@/components/Loading.vue'
	import type { Dock } from '@/model/Dock'
	import { DockService } from '@/service/DockService'
	import AxiosHttpService from '@/service/AxiosHttpService'

	const http = new AxiosHttpService()
	const dockService = new DockService(http as any)

	const Docks = ref<Dock[]>([])
	const loading = ref(false)
	const error = ref<string | null>(null)

	async function loadDocks() {
		loading.value = true
		error.value = null
		try {
			Docks.value = await dockService.getDocks()
		} catch (e: any) {
			console.error('[Dock] failed loading Dock members', e)
			error.value = e?.message ?? String(e)
		} finally {
			loading.value = false
		}
	}

	onMounted(() => { void loadDocks() })
</script>

<template>
	<div>
		<sl-breadcrumb>
		<sl-breadcrumb-item>Dock</sl-breadcrumb-item>
		<sl-breadcrumb-item>Listings</sl-breadcrumb-item>
		</sl-breadcrumb>

		<header>
		<h1 class="title">Dock</h1>
		<p class="subtitle">List of all Docks</p>
		</header>

		<section style="margin-top:1rem">
		<div v-if="loading">
			<Loading/>
		</div>
		<div v-else-if="error" class="error">Error: {{ error }}</div>
		<div v-else>
			<div v-if="Docks.length === 0"> No Docks found. </div>
			<ul v-else>
			<li v-for="s in Docks" :key="s.code">
				{{ s.code || 'no code' }} -
				{{ s.name || 'no name' }} -
        {{ s.location || 'no location' }} -
        {{ s.physicalCharacteristics.length || 'no length' }} -
        {{ s.physicalCharacteristics.depth || 'no depth' }} -
        {{ s.physicalCharacteristics.draft || 'no draft' }} -
        {{ s.supportedVesselTypes.map(v => v.name).join(', ') || 'no supported vessel types' }}
			</li>
			</ul>
		</div>
		</section>
	</div>
</template>