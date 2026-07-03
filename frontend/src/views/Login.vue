<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-card">
        <div class="card-header">
          <div class="logo-icon">▣</div>
          <h2>Admin</h2>
          <p class="subtitle">用户管理系统</p>
        </div>

        <form @submit.prevent="handleLogin" class="login-form">
          <div class="input-group">
            <label>邮箱地址</label>
            <div class="input-wrapper">
              <span class="input-icon">✉</span>
              <input
                v-model="form.email"
                type="email"
                placeholder="请输入邮箱"
                required
                autocomplete="email"
              />
            </div>
          </div>

          <div class="input-group">
            <label>登录密码</label>
            <div class="input-wrapper">
              <span class="input-icon">🔒</span>
              <input
                v-model="form.password"
                type="password"
                placeholder="请输入密码"
                required
                autocomplete="current-password"
              />
            </div>
          </div>

          <div v-if="errorMsg" class="error-msg">
            <span class="error-icon">⚠</span>
            {{ errorMsg }}
          </div>

          <button type="submit" :disabled="loading" class="btn-login">
            <span v-if="loading" class="spinner"></span>
            {{ loading ? '验证中...' : '登 录' }}
          </button>
        </form>

        <div class="card-footer">
          <span class="footer-text">还没有账号？</span>
          <a href="#" @click.prevent="showRegister = true">立即注册</a>
        </div>
      </div>
    </div>

    <div v-if="showRegister" class="modal-overlay" @click.self="showRegister = false">
      <div class="register-card">
        <div class="card-header">
          <h3>创建账号</h3>
          <p class="subtitle">注册一个新的用户账号</p>
        </div>

        <form @submit.prevent="handleRegister">
          <div class="input-group">
            <label>用户名</label>
            <input
              v-model="registerForm.userName"
              type="text"
              placeholder="请输入用户名"
              required
            />
          </div>

          <div class="input-group">
            <label>邮箱地址</label>
            <input
              v-model="registerForm.email"
              type="email"
              placeholder="请输入邮箱"
              required
            />
          </div>

          <div class="input-group">
            <label>登录密码</label>
            <input
              v-model="registerForm.password"
              type="password"
              placeholder="至少6位密码"
              required
            />
          </div>

          <div v-if="registerError" class="error-msg">
            <span class="error-icon">⚠</span>
            {{ registerError }}
          </div>

          <div class="btn-group">
            <button type="submit" :disabled="registerLoading" class="btn-primary">
              <span v-if="registerLoading" class="spinner"></span>
              {{ registerLoading ? '注册中...' : '创建账号' }}
            </button>
            <button type="button" class="btn-ghost" @click="showRegister = false">取消</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import request from '@/utils/request'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const errorMsg = ref('')
const showRegister = ref(false)

const form = reactive({ email: '', password: '' })
const registerForm = reactive({ userName: '', email: '', password: '' })
const registerLoading = ref(false)
const registerError = ref('')

async function handleLogin() {
  loading.value = true
  errorMsg.value = ''
  try {
    await authStore.login(form)
    router.push('/')
  } catch (err: any) {
    errorMsg.value = err.message || '登录失败'
  } finally {
    loading.value = false
  }
}

async function handleRegister() {
  registerLoading.value = true
  registerError.value = ''
  try {
    await request.post('/users', {
      userName: registerForm.userName,
      email: registerForm.email,
      password: registerForm.password,
      role: 'User'
    })
    showRegister.value = false
    Object.assign(registerForm, { userName: '', email: '', password: '' })
    alert('注册成功，请登录')
  } catch (err: any) {
    registerError.value = err.message || '注册失败'
  } finally {
    registerLoading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 50%, #f1f5f9 100%);
  position: relative;
  overflow: hidden;
}

.login-page::before {
  content: '';
  position: absolute;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.06) 0%, transparent 70%);
  top: -200px;
  right: -200px;
  border-radius: 50%;
}

.login-page::after {
  content: '';
  position: absolute;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.04) 0%, transparent 70%);
  bottom: -100px;
  left: -100px;
  border-radius: 50%;
}

.login-container {
  position: relative;
  z-index: 1;
  width: 420px;
  max-width: 92vw;
}

.login-card, .register-card {
  background: #fff;
  border-radius: 20px;
  padding: 40px 36px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04), 0 8px 40px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
}

.card-header {
  text-align: center;
  margin-bottom: 32px;
}

.logo-icon {
  width: 52px;
  height: 52px;
  margin: 0 auto 16px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  font-weight: 700;
  box-shadow: 0 4px 14px rgba(99, 102, 241, 0.25);
}

.card-header h2 {
  font-size: 22px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.card-header h3 {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.subtitle {
  color: #94a3b8;
  font-size: 14px;
  font-weight: 400;
}

.input-group {
  margin-bottom: 20px;
}

.input-group label {
  display: block;
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 600;
  color: #475569;
  letter-spacing: 0.01em;
}

.input-wrapper {
  position: relative;
}

.input-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 16px;
  pointer-events: none;
  opacity: 0.5;
}

.input-wrapper input {
  padding-left: 42px;
}

input {
  width: 100%;
  padding: 12px 16px;
  background: #f8fafc;
  border: 1.5px solid #e2e8f0;
  border-radius: 12px;
  font-size: 14px;
  color: #1e293b;
  transition: all 0.2s;
  outline: none;
}

input:focus {
  border-color: #6366f1;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.08);
}

input::placeholder {
  color: #cbd5e1;
}

.error-msg {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 10px;
  color: #dc2626;
  font-size: 13px;
  margin-bottom: 20px;
}

.error-icon {
  font-size: 14px;
  flex-shrink: 0;
}

.btn-login {
  width: 100%;
  padding: 13px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  border: none;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-top: 4px;
  box-shadow: 0 4px 14px rgba(99, 102, 241, 0.3);
}

.btn-login:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(99, 102, 241, 0.35);
}

.btn-login:active {
  transform: translateY(0);
}

.btn-login:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.card-footer {
  margin-top: 24px;
  text-align: center;
  font-size: 14px;
}

.footer-text {
  color: #94a3b8;
}

.card-footer a {
  font-weight: 600;
  margin-left: 4px;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.4);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.register-card {
  width: 420px;
  max-width: 92vw;
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(16px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.btn-group {
  display: flex;
  gap: 12px;
  margin-top: 4px;
}

.btn-primary {
  flex: 1;
  padding: 12px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  border: none;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  box-shadow: 0 4px 14px rgba(99, 102, 241, 0.3);
}

.btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(99, 102, 241, 0.35);
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.btn-ghost {
  flex: 1;
  padding: 12px;
  background: #f1f5f9;
  color: #64748b;
  border: 1.5px solid #e2e8f0;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-ghost:hover {
  background: #e2e8f0;
  color: #475569;
}
</style>