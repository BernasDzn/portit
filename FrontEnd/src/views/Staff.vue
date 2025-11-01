<script setup lang="ts">
	import { ref, onMounted } from 'vue'
	import Loading from '@/components/Loading.vue'
	import type { Staff } from '@/model/Staff'
	import { StaffService } from '@/service/StaffService'
	import AxiosHttpService from '@/service/AxiosHttpService'
	import { StaffMapper } from '@/model/mappers/StaffMapper'

	const http = new AxiosHttpService()
	const staffService = new StaffService(http as any)

	const staffs = ref<Staff[]>([])
	const loading = ref(false)
	const error = ref<string | null>(null)

	async function loadStaffs() {
		loading.value = true
		error.value = null
		try {
			staffs.value = await staffService.getStaffs()
		} catch (e: any) {
			console.error('[Staff] failed loading staff members', e)
			error.value = e?.message ?? String(e)
		} finally {
			loading.value = false
		}
	}

	onMounted(() => { void loadStaffs() })
</script>

<template>
	<div>
		<sl-breadcrumb>
		<sl-breadcrumb-item>Staff</sl-breadcrumb-item>
		<sl-breadcrumb-item>Listings</sl-breadcrumb-item>
		</sl-breadcrumb>

		<header>
		<h1 class="title">Staff</h1>
		<p class="subtitle">List of all staff members</p>
		</header>

		<section style="margin-top:1rem">
		<div v-if="loading">
			<Loading/>
		</div>
		<div v-else-if="error" class="error">Error: {{ error }}</div>
		<div v-else>
			<div v-if="staffs.length === 0"> No staff members found. </div>
			<!--
				Ofc here should go a table that we populate dynamically but getting the
				http requests working took so long this started to PMO
			-->
			<ul v-else>
			<li v-for="s in staffs" :key="s.mechanograficNumber">
				{{ s.mechanograficNumber || 'well this is unexpected... this staff doesnt have a mechanograficNumber... how...' }} -
				{{ s.name || 'well this is unexpected... this staff doesnt have a name.' }}
			</li>
			</ul>
		</div>
		</section>
	</div>
</template>