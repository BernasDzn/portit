<script>
import FormField from '@/components/crud/FormField.vue'
import EntityDropdown from '@/components/crud/EntityDropdown.vue'
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue'

export default {
  name: 'GeneralFields',
  components: { FormField, EntityDropdown, OperationalWindowPicker },
  props: {
    t: { type: Function, required: true },
    genericResource: { type: Object, required: true },
    statuses: { type: Array, required: true },
    qualificationService: { type: Object, required: true }
  }
}
</script>

<template>
  <p class="section-title">{{ t('physicalResource.generalFields') }}</p>
  <div class="group">
    <div class="form">
      <FormField
        required
        class="field"
        :name="`${t('physicalResource.fields.code.title')}*`"
        v-model="genericResource.code"
        pattern="^[a-zA-Z0-9]+$"
      />
      <span class="section-divider"></span>

      <FormField
        required
        class="field"
        :name="`${t('physicalResource.fields.description.title')}*`"
        v-model="genericResource.description"
      />
      <span class="section-divider"></span>

        <EntityDropdown
            class="field-dropdown"
            :name="`${t('physicalResource.fields.status.title')}*`"
            v-model="genericResource.status"
            :items="statuses"
            required
        />

        <span class="section-divider"></span>

        <EntityDropdown
            class="field-dropdown"
            :name="`${t('physicalResource.fields.qualifications.title')}*`"
            v-model="genericResource.qualifications"
            :fetch-function="() => qualificationService.getQualifications()"
            fetch-on-mount
            valueKey="idCode"
            labelKey="idCode"
            multiple
            required
        />

        <span class="section-divider"></span>

        <FormField
            required
            class="field"
            :name="`${t('physicalResource.fields.setupTime.title')}*`"
            v-model="genericResource.setupTime"
            pattern="^[0-9]+$"
        />
    </div>

    <br />

    <OperationalWindowPicker v-model="genericResource.operationalWindow" />
  </div>
</template>

<style scoped>

.section-divider {
  width: 1px;
  margin: 0 1rem;
  background-color: var(--sl-color-neutral-200);
}

.section-title {
  font-size: 0.8rem;
  margin-bottom: 1rem;
  color: var(--sl-color-neutral-400);
}

.form {
  display: flex;
  flex-direction: row;
}

.group {
  margin: 5px;
}
</style>
