<!-- 
	Simple Data Table implementation that receives column names (that map directly to object keys)
	and an array of objects and populates a table accordingly.
-->
<script setup lang="ts">
	type ColumnValues = string

	const props = defineProps<{
		columns: ColumnValues[]
		rows: Record<string, any>[]
		keyField?: string
		emptyText?: string
	}>()

	const keyField = props.keyField ?? '' 
	const emptyText = props.emptyText ?? 'No results'

	function PrettifyColumnNames(column: string) {
		const withSpaces = column.replace(/([A-Z])/g, ' $1').replace(/_/g, ' ')
		return withSpaces.charAt(0).toUpperCase() + withSpaces.slice(1)
	}
</script>

<template>
	<div class="data-table">
		<table class="dt-table">
			
			<thead>
				<tr>
					<th v-for="column in columns">
						{{ PrettifyColumnNames(column) }}
					</th>
				</tr>
			</thead>

			<tbody>
				
				<tr v-if="rows.length === 0"> <!-- Show empty text if no rows for column property -->
					<td :colspan="columns.length" class="dt-empty">{{ emptyText }}</td>
				</tr>

                <tr v-for="row in rows" :key="row[keyField]">
                    <td v-for="column in columns" :key="column">
                        <slot :name="column" :value="row[column]" :row="row">
                            {{ row[column] }}
                        </slot>
                    </td>
                </tr>
                

			</tbody>
		</table>
	</div>
</template>

<!-- Simple scoped css trying to mimick figma's css -->
<style scoped>
	.dt-table {
		width: 100%;
		border-collapse: collapse;
	}

	.dt-table thead {
		font-weight: bold;
		color: rgb(107, 105, 115);
	}

	.dt-table th,
	.dt-table td {
		border-bottom: 1px solid #ddd;
		padding: 1rem;
		text-align: left;
	}

	.dt-empty {
		text-align: center;
		color: #666;
		padding: 1rem;
	}
</style>
