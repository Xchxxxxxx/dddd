<template>
  <div class="users-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">用户管理</h1>
        <p class="page-desc">管理系统中的所有用户账号</p>
      </div>
      <button class="btn-add" @click="openCreateDialog">
        <span class="btn-icon">＋</span>
        新增用户
      </button>
    </div>

    <div class="stats-row">
      <div class="stat-card">
        <div class="stat-icon stat-icon--purple">◎</div>
        <div class="stat-content">
          <p class="stat-value">{{ users.length }}</p>
          <p class="stat-label">用户总数</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon--green">◆</div>
        <div class="stat-content">
          <p class="stat-value">{{ users.filter(u => u.role === 'Admin').length }}</p>
          <p class="stat-label">管理员</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon--blue">◈</div>
        <div class="stat-content">
          <p class="stat-value">{{ users.filter(u => u.role === 'User').length }}</p>
          <p class="stat-label">普通用户</p>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>用户</th>
              <th>邮箱</th>
              <th>角色</th>
              <th>创建时间</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="5" class="empty-cell">
                <div class="loading-state">
                  <span class="loading-spinner"></span>
                  加载中...
                </div>
              </td>
            </tr>
            <tr v-else-if="users.length === 0">
              <td colspan="5" class="empty-cell">
                <div class="empty-state">
                  <span class="empty-icon">◎</span>
                  <p>暂无用户数据</p>
                </div>
              </td>
            </tr>
            <tr v-for="user in users" :key="user.id" class="row">
              <td>
                <div class="user-cell">
                  <div class="user-avatar" :class="user.role === 'Admin' ? 'avatar-admin' : 'avatar-user'">
                    {{ user.userName?.charAt(0) || '?' }}
                  </div>
                  <span class="user-cell-name">{{ user.userName }}</span>
                </div>
              </td>
              <td>
                <span class="email-text">{{ user.email }}</span>
              </td>
              <td>
                <span class="role-badge" :class="user.role === 'Admin' ? 'badge-admin' : 'badge-user'">
                  {{ user.role === 'Admin' ? '管理员' : '普通用户' }}
                </span>
              </td>
              <td>
                <span class="date-text">{{ formatDate(user.createdAt) }}</span>
              </td>
              <td>
                <div class="action-btns">
                  <button class="btn-action btn-edit" @click="openEditDialog(user)">
                    <span>✎</span> 编辑
                  </button>
                  <button class="btn-action btn-delete" @click="handleDelete(user.id)">
                    <span>✕</span> 删除
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="dialogVisible" class="modal-overlay" @click.self="closeDialog">
      <div class="dialog-card">
        <div class="dialog-header">
          <h3>{{ isEdit ? '编辑用户' : '新增用户' }}</h3>
          <button class="btn-close" @click="closeDialog">✕</button>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="input-group">
            <label>用户名</label>
            <input v-model="form.userName" type="text" placeholder="请输入用户名" required />
          </div>

          <div class="input-group">
            <label>邮箱地址</label>
            <input v-model="form.email" type="email" placeholder="请输入邮箱" :disabled="isEdit" required />
          </div>

          <div class="input-group" v-if="!isEdit">
            <label>登录密码</label>
            <input v-model="form.password" type="password" placeholder="至少6位密码" required />
          </div>

          <div class="input-group">
            <label>用户角色</label>
            <select v-model="form.role" class="select-input">
              <option value="User">普通用户</option>
              <option value="Admin">管理员</option>
            </select>
          </div>

          <div v-if="submitError" class="error-msg">
            <span class="error-icon">⚠</span>
            {{ submitError }}
          </div>

          <div class="btn-group">
            <button type="submit" :disabled="submitting" class="btn-primary">
              <span v-if="submitting" class="spinner"></span>
              {{ submitting ? '保存中...' : isEdit ? '保存修改' : '创建用户' }}
            </button>
            <button type="button" class="btn-ghost" @click="closeDialog">取消</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import request from '@/utils/request'

interface User {
  id: string
  userName: string
  email: string
  role: string
  createdAt: string
}

const users = ref<User[]>([])
const loading = ref(true)
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const submitError = ref('')
const editingId = ref('')

const form = reactive({
  userName: '',
  email: '',
  password: '',
  role: 'User'
})

onMounted(() => {
  fetchUsers()
})

async function fetchUsers() {
  loading.value = true
  try {
    const res = await request.get('/users')
    const data = res?.data
    users.value = Array.isArray(data) ? data : data?.items ?? []
  } catch {
    users.value = []
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  isEdit.value = false
  editingId.value = ''
  submitError.value = ''
  Object.assign(form, { userName: '', email: '', password: '', role: 'User' })
  dialogVisible.value = true
}

function openEditDialog(user: User) {
  isEdit.value = true
  editingId.value = user.id
  submitError.value = ''
  Object.assign(form, {
    userName: user.userName,
    email: user.email,
    password: '',
    role: user.role
  })
  dialogVisible.value = true
}

function closeDialog() {
  dialogVisible.value = false
}

async function handleSubmit() {
  submitting.value = true
  submitError.value = ''
  try {
    if (isEdit.value) {
      await request.put(`/users/${editingId.value}`, {
        userName: form.userName,
        email: form.email,
        role: form.role
      })
    } else {
      await request.post('/users', {
        userName: form.userName,
        email: form.email,
        password: form.password,
        role: form.role
      })
    }
    closeDialog()
    await fetchUsers()
  } catch (err: any) {
    submitError.value = err.message || '操作失败'
  } finally {
    submitting.value = false
  }
}

async function handleDelete(id: string) {
  if (!confirm('确定要删除此用户吗？')) return
  try {
    await request.delete(`/users/${id}`)
    await fetchUsers()
  } catch {
    alert('删除失败')
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}
</script>

<style scoped>
.users-page {
  max-width: 100%;
}

.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.page-desc {
  font-size: 14px;
  color: #94a3b8;
}

.btn-add {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 20px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  border: none;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 4px 14px rgba(99, 102, 241, 0.3);
}

.btn-add:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(99, 102, 241, 0.35);
}

.btn-icon {
  font-size: 16px;
  font-weight: 400;
}

.stats-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  background: #fff;
  border: 1px solid #f1f5f9;
  border-radius: 16px;
  padding: 20px 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.02);
  transition: box-shadow 0.2s;
}

.stat-card:hover {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.04);
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
}

.stat-icon--purple {
  background: #f3f0ff;
  color: #6366f1;
}

.stat-icon--green {
  background: #ecfdf5;
  color: #10b981;
}

.stat-icon--blue {
  background: #eff6ff;
  color: #3b82f6;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #1e293b;
  line-height: 1.2;
}

.stat-label {
  font-size: 13px;
  color: #94a3b8;
  margin-top: 2px;
}

.card {
  background: #fff;
  border: 1px solid #f1f5f9;
  border-radius: 16px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.02);
  overflow: hidden;
}

.table-wrapper {
  overflow-x: auto;
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table thead {
  background: #f8fafc;
}

.table th {
  padding: 14px 20px;
  text-align: left;
  font-size: 13px;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  border-bottom: 1px solid #f1f5f9;
}

.table td {
  padding: 16px 20px;
  font-size: 14px;
  color: #334155;
  border-bottom: 1px solid #f8fafc;
}

.row {
  transition: background 0.15s;
}

.row:hover {
  background: #fafafa;
}

.row:last-child td {
  border-bottom: none;
}

.user-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 700;
  color: #fff;
}

.avatar-admin {
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
}

.avatar-user {
  background: linear-gradient(135deg, #3b82f6, #60a5fa);
}

.user-cell-name {
  font-weight: 600;
  color: #1e293b;
}

.email-text {
  color: #64748b;
}

.role-badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}

.badge-admin {
  background: #f3f0ff;
  color: #6366f1;
}

.badge-user {
  background: #eff6ff;
  color: #3b82f6;
}

.date-text {
  color: #94a3b8;
  font-size: 13px;
}

.action-btns {
  display: flex;
  gap: 8px;
}

.btn-action {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 6px 12px;
  border: none;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-action span {
  font-size: 12px;
}

.btn-edit {
  background: #f0f9ff;
  color: #3b82f6;
}

.btn-edit:hover {
  background: #dbeafe;
}

.btn-delete {
  background: #fef2f2;
  color: #ef4444;
}

.btn-delete:hover {
  background: #fee2e2;
}

.empty-cell {
  text-align: center;
  padding: 48px 20px !important;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  color: #94a3b8;
}

.empty-icon {
  font-size: 32px;
  opacity: 0.4;
}

.empty-state p {
  font-size: 14px;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  color: #94a3b8;
  font-size: 14px;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #e2e8f0;
  border-top-color: #6366f1;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
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

.dialog-card {
  width: 440px;
  max-width: 92vw;
  background: #fff;
  border-radius: 20px;
  padding: 32px;
  box-shadow: 0 8px 40px rgba(0, 0, 0, 0.08);
  border: 1px solid #f1f5f9;
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

.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
}

.dialog-header h3 {
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
}

.btn-close {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  background: #f1f5f9;
  color: #94a3b8;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
}

.btn-close:hover {
  background: #e2e8f0;
  color: #64748b;
}

.input-group {
  margin-bottom: 18px;
}

.input-group label {
  display: block;
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 600;
  color: #475569;
}

input, .select-input {
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

input:focus, .select-input:focus {
  border-color: #6366f1;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.08);
}

input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

input::placeholder {
  color: #cbd5e1;
}

.select-input {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%2394a3b8' stroke-width='2'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 16px center;
  padding-right: 40px;
  cursor: pointer;
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
  margin-bottom: 18px;
}

.error-icon {
  font-size: 14px;
  flex-shrink: 0;
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

.spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}
</style>