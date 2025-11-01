<script setup lang="ts">
import DataTable from '@/components/DataTable.vue'
import { ref, onMounted } from 'vue'
import Loading from '@/components/Loading.vue'
import type { Staff } from '@/model/Staff'
import { StaffService } from '@/service/StaffService'
import AxiosHttpService from '@/service/AxiosHttpService'
import ErrorHandler from '@/components/ErrorHandler.vue'

const http = new AxiosHttpService()
const staffService = new StaffService(http as any)

const staffs = ref<Staff[]>([])
const loading = ref(false)
const error = ref<Error | null>(null)

async function loadStaffs() {

	loading.value = true
	error.value = null
	try {
		staffs.value = await staffService.getStaffs()
	} catch (e: any) {
		console.error('[Staff] failed loading staff members', e)
		error.value = e;
	} finally {
		loading.value = false
	}
}

onMounted(() => { loadStaffs() });

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
			<Loading message="Loading staff list…" />
		</div>
		<div v-else-if="error"><ErrorHandler :error-object="error" /></div>
		<div v-else>
			<DataTable
				:columns="['mechanographicNumber', 'name', 'email', 'phoneNumber']"
				:rows="staffs"
			/>
		</div>
		</section>
	</div>
</template>