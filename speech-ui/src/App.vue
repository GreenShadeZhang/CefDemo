<script setup lang="ts">
import { ref } from 'vue'

// Define reactive variables
const recognizedText = ref('')
const isRecording = ref(false)

// Define methods to interact with WPF bridge
const startRecording = () => {
  isRecording.value = true
  // Call WPF bridge method
  window.speechBridge?.startRecording()
}

const stopRecording = () => {
  isRecording.value = false
  window.speechBridge?.stopRecording()
}

// Define global method for WPF to call
declare global {
  interface Window {
    speechBridge: any;
    updateRecognizedText: (text: string) => void;
  }
}

// Mount callback to set up bridge
window.updateRecognizedText = (text: string) => {
  recognizedText.value += text + ' '
}
</script>

<template>
  <div class="container">
    <header>
      <h1>Speech Recognition</h1>
      <div class="status" :class="{ active: isRecording }">
        {{ isRecording ? 'Recording...' : 'Ready' }}
      </div>
    </header>

    <main>
      <div class="controls">
        <button 
          @click="startRecording" 
          :disabled="isRecording"
          class="btn primary">
          Start Recording
        </button>
        <button 
          @click="stopRecording" 
          :disabled="!isRecording"
          class="btn secondary">
          Stop Recording
        </button>
      </div>

      <div class="text-display">
        <h3>Recognized Text:</h3>
        <div class="text-content">
          {{ recognizedText }}
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.status {
  padding: 0.5rem 1rem;
  border-radius: 4px;
  background: #e0e0e0;
}

.status.active {
  background: #4CAF50;
  color: white;
}

.controls {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn.primary {
  background: #1867C0;
  color: white;
}

.btn.secondary {
  background: #757575;
  color: white;
}

.text-display {
  background: #f5f5f5;
  border-radius: 8px;
  padding: 1.5rem;
}

.text-content {
  min-height: 200px;
  margin-top: 1rem;
  white-space: pre-wrap;
  line-height: 1.5;
}
</style>