<template>
  <div class="layout">
    <header class="header">
      <div class="header-inner">
        <div class="brand">
          <span class="brand-icon">▣</span>
          <span class="brand-text">Admin</span>
        </div>

        <nav class="nav">
          <router-link to="/users" class="nav-link" active-class="nav-link--active">
            <span class="nav-icon">◎</span>
            用户管理
          </router-link>
        </nav>

        <div class="header-right">
          <div class="user-info">
            <div class="avatar">{{ authStore.user?.userName?.charAt(0) || 'U' }}</div>
            <span class="user-name">{{ authStore.user?.userName }}</span>
          </div>
          <button class="btn-logout" @click="handleLogout">
            <span class="logout-icon">↪</span>
            退出
          </button>
        </div>
      </div>
    </header>

    <main class="main">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.layout {
  min-height: 100vh;
  background: #f8fafc;
}

.header {
  background: #fff;
  border-bottom: 1px solid #f1f5f9;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.03);
}

.header-inner {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px;
  height: 60px;
  display: flex;
  align-items: center;
  gap: 32px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
}

.brand-icon {
  width: 34px;
  height: 34px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  font-weight: 700;
}

.brand-text {
  font-size: 17px;
  font-weight: 700;
  color: #1e293b;
  letter-spacing: -0.01em;
}

.nav {
  display: flex;
  gap: 4px;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 500;
  color: #64748b;
  transition: all 0.2s;
  cursor: pointer;
}

.nav-link:hover {
  background: #f8fafc;
  color: #475569;
}

.nav-link--active {
  background: #f1f5f9;
  color: #6366f1;
}

.nav-icon {
  font-size: 16px;
}

.header-right {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 16px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.avatar {
  width: 34px;
  height: 34px;
  border-radius: 10px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
}

.user-name {
  font-size: 14px;
  font-weight: 500;
  color: #334155;
}

.btn-logout {
  display: flex;
  align-items: center;
  gap: 5px;
  padding: 8px 14px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-logout:hover {
  background: #fee2e2;
  border-color: #fecaca;
  color: #dc2626;
}

.logout-icon {
  font-size: 14px;
}

.main {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;
}
</style>