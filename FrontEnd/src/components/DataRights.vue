<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getApiBase } from '@/config';
import { useSession } from '@/composables/session';
import jsPDF from 'jspdf';
import router from '@/router';
import type { IAuthService } from '@/service/IService/IAuthService';
import TYPES from '@/inversify/types';
import { container } from '@/inversify.config';

interface UserData {
  sub: string;
  isActive: boolean;
  role: number;
  email: string;
}

const session = useSession();
const userData = ref<UserData | null>(null);
const loading = ref(false);
const showDeletionWarning = ref(false);
const showCountdownDialog = ref(false);
const countdownSeconds = ref(60);
let countdownInterval: number | null = null;

const authService = container.get<IAuthService>(TYPES.authService);    

// Fetch user data
const fetchUserData = async () => {
  loading.value = true;
  try {
    const response = await fetch(`${getApiBase()}/SystemUser/myData`, {
      headers: {
        'Authorization': `Bearer ${session.authToken}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error('Failed to fetch user data');
    }

    const data = await response.json();
    userData.value = data;
  } catch (error) {
    console.error('Error fetching user data:', error);
    showNotification('Failed to load user data', 'danger');
  } finally {
    loading.value = false;
  }
};

// Download data as JSON
const downloadJSON = () => {
  if (!userData.value) return;

  const dataStr = JSON.stringify(userData.value, null, 2);
  const blob = new Blob([dataStr], { type: 'application/json' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = `my-data-${new Date().toISOString().split('T')[0]}.json`;
  link.click();
  URL.revokeObjectURL(url);

  showNotification('Your data has been downloaded as JSON', 'success');
};

// Download data as PDF
const downloadPDF = () => {
  if (!userData.value) return;

  const doc = new jsPDF();
  
  // Title
  doc.setFontSize(18);
  doc.text('Personal Data Export', 20, 20);
  
  // Timestamp
  doc.setFontSize(10);
  doc.text(`Generated: ${new Date().toLocaleString()}`, 20, 30);
  
  // Data
  doc.setFontSize(12);
  let y = 50;
  
  doc.text('Email:', 20, y);
  doc.text(userData.value.email, 60, y);
  y += 10;
  
  doc.text('Sub (ID):', 20, y);
  doc.text(userData.value.sub, 60, y);
  y += 10;
  
  doc.text('Role:', 20, y);
  doc.text(String(userData.value.role), 60, y);
  y += 10;
  
  doc.text('Active:', 20, y);
  doc.text(userData.value.isActive ? 'Yes' : 'No', 60, y);
  
  // Footer
  doc.setFontSize(8);
  doc.text('This data export is provided in compliance with GDPR data portability requirements.', 20, 280);
  
  doc.save(`my-data-${new Date().toISOString().split('T')[0]}.pdf`);

  showNotification('Your data has been downloaded as PDF', 'success');
};

// Request data deletion
const requestDeletion = () => {
    showDeletionWarning.value = true;
};

const confirmDeletion = () => {
    showDeletionWarning.value = false;
    showCountdownDialog.value = true;
    countdownSeconds.value = 60;
    
    // Start countdown
    countdownInterval = setInterval(() => {
        countdownSeconds.value--;
        
        if (countdownSeconds.value <= 0) {
            if (countdownInterval) {
                clearInterval(countdownInterval);
            }
            executeDeletion();
        }
    }, 1000);
};

const cancelDeletion = () => {
    if (countdownInterval) {
        clearInterval(countdownInterval);
        countdownInterval = null;
    }
    showCountdownDialog.value = false;
    countdownSeconds.value = 60;
};

const executeDeletion = async () => {
    loading.value = true;
    try {
        const response = await fetch(`${getApiBase()}/SystemUser/myData`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${session.authToken}`,
                'Content-Type': 'application/json'
            }
		});

        if (!response.ok) {
            throw new Error('Failed to delete user data');
        }
        
        showNotification('Your data has been deleted. You will be logged out now.', 'success');
        
        // Log out the user
        setTimeout(() => {
            authService.logout();
            router.push({ name: 'login' });
        }, 2000);
    } catch (error) {
        console.error('Error deleting user data:', error);
        showNotification('Failed to delete data. Please try again.', 'danger');
    } finally {
        loading.value = false;
        showCountdownDialog.value = false;
    }
};

// Show notification
const showNotification = (message: string, variant: string) => {
  const alert = Object.assign(document.createElement('sl-alert'), {
    variant,
    closable: true,
    duration: 5000,
    innerHTML: `
      <sl-icon name="info-circle" slot="icon"></sl-icon>
      ${message}
    `
  });

  document.body.append(alert);
  alert.toast();
};

onMounted(() => {
  fetchUserData();
});
</script>

<template>
  <div class="data-rights">
    <sl-card>
      <div slot="header" class="card-header">
        <sl-icon name="shield-lock"></sl-icon>
        <h2>Data Rights & Privacy</h2>
      </div>

      <div v-if="loading" class="loading-state">
        <sl-spinner></sl-spinner>
        <p>Loading your data...</p>
      </div>

      <div v-else-if="userData" class="data-rights-content">
        
        <!-- Current Data Display -->
        <section class="data-section">
          <h3>Your Personal Data</h3>
          <div class="data-display">
            <div class="data-field">
              <label>Email Address:</label>
              <span>{{ userData.email }}</span>
            </div>
            <div class="data-field">
              <label>User ID (Sub):</label>
              <span class="monospace">{{ userData.sub }}</span>
            </div>
          </div>
        </section>

        <!-- Data Export -->
        <section class="data-section">
          <h3><sl-icon name="download"></sl-icon> Export Your Data</h3>
          <p class="section-description">
            Download a complete copy of your stored personal data in your preferred format.
          </p>
          <div class="action-buttons">
            <sl-button variant="primary" @click="downloadJSON">
              <sl-icon slot="prefix" name="file-earmark-code"></sl-icon>
              Download as JSON
            </sl-button>
            <sl-button variant="primary" @click="downloadPDF">
              <sl-icon slot="prefix" name="file-earmark-pdf"></sl-icon>
              Download as PDF
            </sl-button>
          </div>
        </section>

        <!-- Data Deletion -->
        <section class="data-section">
          <h3><sl-icon name="trash"></sl-icon> Delete Your Data</h3>
          <p class="section-description">
            Request deletion of your personal data.
          </p>
          <sl-button variant="danger" @click="requestDeletion">
            <sl-icon slot="prefix" name="exclamation-triangle"></sl-icon>
            Request Data Deletion
          </sl-button>
        </section>
      </div>
    </sl-card>

    <!-- Deletion Warning Dialog -->
    <sl-dialog 
      :open="showDeletionWarning" 
      label="Permanent Data Deletion" 
      @sl-request-close="showDeletionWarning = false"
      class="deletion-warning-dialog"
    >
      <div class="dialog-content">
        <sl-alert variant="danger" open>
          <sl-icon slot="icon" name="exclamation-octagon"></sl-icon>
          <strong>This action cannot be undone!</strong>
        </sl-alert>

        <div class="warning-content">
          <h4>You are about to request permanent deletion of your personal data.</h4>
          
          <p><strong>What will happen:</strong></p>
          <ul>
            <li>All your personal information will be permanently deleted from our systems</li>
            <li>Your account will be deactivated immediately</li>
            <li>You will be logged out and cannot access this account again</li>
            <li>This action is irreversible and cannot be undone</li>
            <li>Any associated data and history will be lost</li>
          </ul>

          <p><strong>Before you proceed:</strong></p>
          <ul>
            <li>Make sure you have downloaded any data you wish to keep</li>
            <li>Consider if you truly want to delete your account permanently</li>
            <li>Remember that you will be logged out in 60 seconds after confirmation</li>
          </ul>
        </div>
      </div>

      <div slot="footer" class="dialog-footer">
        <sl-button variant="default" @click="showDeletionWarning = false">Cancel</sl-button>
        <sl-button variant="danger" @click="confirmDeletion">
          <sl-icon slot="prefix" name="trash"></sl-icon>
          Yes, Delete My Data Permanently
        </sl-button>
      </div>
    </sl-dialog>

    <!-- Countdown Dialog -->
    <sl-dialog 
      :open="showCountdownDialog" 
      label="Data Deletion in Progress" 
      @sl-request-close="() => {}"
      class="countdown-dialog"
      no-header
    >
      <div class="countdown-content">
        <sl-icon name="hourglass-split" class="countdown-icon"></sl-icon>
        
        <h3>Your data will be deleted in:</h3>
        
        <div class="countdown-display">
          {{ countdownSeconds }}
        </div>
        
        <p class="countdown-text">seconds</p>
        
        <sl-alert variant="danger" open>
          <sl-icon slot="icon" name="info-circle"></sl-icon>
          You will be automatically logged out after deletion.
        </sl-alert>
      </div>

      <p class="cancel-text">Changed your mind?</p>
      <div slot="footer" class="dialog-footer">
        <sl-button variant="default" @click="cancelDeletion">
          <sl-icon slot="prefix" name="x-circle"></sl-icon>
          Cancel Deletion
        </sl-button>
      </div>
    </sl-dialog>
  </div>
</template>

<style scoped>
.data-rights {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 10px;
}

.card-header sl-icon {
  font-size: 1.5rem;
  color: var(--sl-color-primary-600);
}

.card-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  padding: 40px;
}

.data-rights-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.data-section {
  border-bottom: 1px solid var(--sl-color-neutral-200);
  padding-bottom: 20px;
}

.data-section:last-child {
  border-bottom: none;
}

.data-section h3 {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0 0 15px 0;
  color: var(--sl-color-neutral-700);
  font-size: 1.1rem;
}

.section-description {
  color: var(--sl-color-neutral-600);
  margin-bottom: 15px;
  font-size: 0.95rem;
}

.data-display {
  background: var(--sl-color-neutral-50);
  border-radius: 8px;
  padding: 15px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.data-field {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.data-field label {
  font-weight: 600;
  color: var(--sl-color-neutral-700);
}

.data-field span {
  color: var(--sl-color-neutral-900);
}

.monospace {
  font-family: 'Courier New', monospace;
  font-size: 0.9rem;
  background: var(--sl-color-neutral-100);
  padding: 4px 8px;
  border-radius: 4px;
}

.action-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.request-log {
  background: var(--sl-color-neutral-50);
  border-radius: 8px;
  padding: 15px;
  max-height: 200px;
  overflow-y: auto;
}

.log-entry {
  padding: 8px 0;
  border-bottom: 1px solid var(--sl-color-neutral-200);
  font-size: 0.9rem;
  color: var(--sl-color-neutral-700);
  font-family: monospace;
}

.log-entry:last-child {
  border-bottom: none;
}

.dialog-footer {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

.deletion-warning-dialog .warning-content {
  margin-top: 15px;
}

.deletion-warning-dialog h4 {
  color: var(--sl-color-danger-600);
  margin: 15px 0;
  font-size: 1.1rem;
}

.deletion-warning-dialog ul {
  margin: 10px 0;
  padding-left: 20px;
}

.deletion-warning-dialog li {
  margin: 8px 0;
  color: var(--sl-color-neutral-700);
}

.countdown-dialog .countdown-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 30px 20px;
  text-align: center;
}

.countdown-icon {
  font-size: 4rem;
  color: var(--sl-color-danger-600);
  margin-bottom: 20px;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.countdown-content h3 {
  margin: 0 0 20px 0;
  color: var(--sl-color-neutral-700);
  font-size: 1.3rem;
}

.countdown-display {
  font-size: 5rem;
  font-weight: bold;
  color: var(--sl-color-danger-600);
  line-height: 1;
  margin: 20px 0;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
}

.countdown-text {
  font-size: 1.2rem;
  color: var(--sl-color-neutral-600);
  margin: 0 0 25px 0;
}

.cancel-text {
    margin: 20px 0 0 0;
    color: var(--sl-color-neutral-600);
    font-size: 0.95rem;
    text-align: right;
}

@media (max-width: 600px) {
  .data-rights {
    padding: 10px;
  }

  .data-field {
    flex-direction: column;
    align-items: flex-start;
    gap: 5px;
  }

  .action-buttons {
    flex-direction: column;
  }

  .action-buttons sl-button {
    width: 100%;
  }
  
  .countdown-display {
    font-size: 4rem;
  }
}
</style>
