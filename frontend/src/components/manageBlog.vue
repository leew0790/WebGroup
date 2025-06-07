<template>
  <component :is="layoutComponent">
    <div :class="['container', role === 'Admin' ? 'admin-tone' : '']">
      <h2 class="page-title">📚 News</h2>
      <!-- 🔍 Search Blog -->
      <div class="search-section">
        <input v-model="searchKeyword" type="text" placeholder="🔍 Search blog by title..." class="search-input"
          @keyup.enter="handleSearch" />
        <button class="search-btn" @click="handleSearch">Searching</button>
        <button class="reset-btn" @click="resetSearch">Reload</button>
      </div>


      <!-- Fixed icon cho New Blog -->
      <div v-if="role !== 'Admin'" class="new-blog-icon" @click="toggleCreateForm">
        <span v-if="!showCreateForm">📝 New Blog</span>
        <span v-else>✖ Close</span>
      </div>

      <!-- Create Blog Form Overlay -->
      <div v-if="role !== 'Admin' && showCreateForm" class="create-blog-overlay">
        <!-- Nền mờ -->
        <div class="overlay-bg" @click="toggleCreateForm"></div>

        <!-- Form chính: hiển thị gần top, màu tím đậm -->
        <div class="create-blog-form">
          <h3 class="form-title">📝 What do you think?</h3>
          <input v-model="newBlog.title" placeholder="Title" class="input" />
          <textarea v-model="newBlog.content" placeholder="Content" class="input"></textarea>
          <input type="file" @change="handleFileChange" class="input-file" />
          <button @click="createNewBlog" class="btn btn-primary">POST</button>
        </div>
      </div>

      <!-- Blog List -->
      <div class="blog-list">
        <div v-for="blog in blogs" :key="blog.id" class="blog-card">
          <!-- Header: Tác giả và Thời gian -->
          <div class="blog-header">
            <div class="blog-meta">
              ✍️ <span class="author">{{ blog.user }}</span> | 🕒 <span class="time">{{ formatDate(blog.createdAt)
              }}</span>
            </div>
            <div class="blog-actions" v-if="canEdit(blog) || canDelete(blog)">
              <button v-if="canEdit(blog)" @click="editBlog(blog)" class="action-btn edit-btn"
                title="Sửa blog">✏️</button>
              <button v-if="canDelete(blog)" @click="deleteBlog(blog.id)" class="action-btn delete-btn"
                title="Xóa blog">🗑️</button>
            </div>
          </div>

          <!-- Nội dung Blog -->
          <h3 class="blog-title">{{ blog.title }}</h3>
          <p class="blog-content">{{ blog.content }}</p>

          <!-- File đính kèm -->
          <div v-if="blog.url" class="blog-file">
            <img v-if="!blog.url.toLowerCase().endsWith('.pdf')" :src="getFullUrl(blog.url)" alt="Blog Image"
              class="blog-image" />
            <a v-else :href="getFullUrl(blog.url)" target="_blank" class="file-link">📄 Xem file PDF</a>
          </div>

          <!-- Form chỉnh sửa -->
          <div v-if="editingBlog && editingBlog.id === blog.id" class="edit-form">
            <input v-model="editingBlog.title" class="input" />
            <textarea v-model="editingBlog.content" class="input"></textarea>
            <input type="file" @change="handleEditFileChange" class="input-file" />
            <button @click="updateBlogPost" class="btn btn-primary">Update</button>
            <button @click="cancelEdit" class="btn btn-secondary">Cancel</button>
          </div>

          <!-- Comments component -->
          <Comments :blogId="blog.id" :isAdmin="role === 'Admin'" />
        </div>
      </div>
    </div>
  </component>
</template>


<script>

import blogService from '../api/blogService';
import userLayout from './userLayout.vue';
import adminLayout from './adminLayout.vue';
import jwtDecode from 'jwt-decode';
import manageComment from './manageComment.vue';

export default {
  name: 'manageBlog',
  components: {
    userLayout,
    Comments: manageComment
  },
  data() {
    return {
      blogs: [],
      allBlogs: [], // ← NEW: lưu bản gốc để reset tìm kiếm
      searchKeyword: '', 
      newBlog: {
        title: '',
        content: '',
        file: null,
      },
      editingBlog: null,
      role: '',
      layoutComponent: userLayout,
      username: '',
      backendBaseUrl: 'https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net',
      showCreateForm: false  // Biến dùng để toggle hiển thị form tạo blog
    };
  },
  methods: {
    async fetchBlogs() {
      try {
        const res = await blogService.getAllBlogs();
        const sorted = res.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        // Sắp xếp blog theo thời gian tạo giảm dần (mới nhất lên đầu)
        this.blogs = res.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        this.allBlogs = [...sorted];
      } catch (error) {
        console.error(error);
      }
    },

    handleSearch() {
      const keyword = this.searchKeyword.toLowerCase().trim();
      if (!keyword) {
        alert("Vui lòng nhập từ khóa tìm kiếm tiêu đề.");
        return;
      }

      this.blogs = this.allBlogs.filter(blog =>
        blog.title.toLowerCase().includes(keyword)
      );
    },

    resetSearch() {
      this.searchKeyword = '';
      this.blogs = [...this.allBlogs];
    },

    getTokenInfo() {
      const token = localStorage.getItem('token');
      if (token) {
        const decoded = jwtDecode(token);
        this.role = decoded.role;
        this.username = decoded.username || decoded.given_name;
      }
    },
    handleFileChange(event) {
      const file = event.target.files[0];
      if (file && !this.isValidFileFormat(file)) {
        alert('Only images and PDF files are allowed.');
        event.target.value = '';
      } else {
        this.newBlog.file = file;
      }
    },
    handleEditFileChange(event) {
      const file = event.target.files[0];
      if (file && !this.isValidFileFormat(file)) {
        alert('Only images  and PDF files are allowed.');
        event.target.value = '';
      } else {
        this.editingBlog.file = file;
      }
    },
    isValidFileFormat(file) {
      const allowedFormats = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp', 'application/pdf'];
      return allowedFormats.includes(file.type);
    },
    async createNewBlog() {
      if (!this.newBlog.title || !this.newBlog.content) {
        alert('Title and Content fields cannot be empty.');
        return;
      }
      const formData = new FormData();
      formData.append('Title', this.newBlog.title);
      formData.append('Content', this.newBlog.content);
      if (this.newBlog.file) {
        formData.append('File', this.newBlog.file);
      }
      try {
        await blogService.createBlog(formData);
        this.newBlog = { title: '', content: '', file: null };
        await this.fetchBlogs();
        // Ẩn form sau khi tạo mới thành công
        this.showCreateForm = false;
      } catch (error) {
        console.error(error);
      }
    },
    editBlog(blog) {
      this.editingBlog = { ...blog };
    },
    cancelEdit() {
      this.editingBlog = null;
    },
    async updateBlogPost() {
      try {
        const formData = new FormData();
        formData.append('Title', this.editingBlog.title);
        formData.append('Content', this.editingBlog.content);
        if (this.editingBlog.file) {
          formData.append('File', this.editingBlog.file);
        }
        await blogService.updateBlog(this.editingBlog.id, formData);
        this.editingBlog = null;
        await this.fetchBlogs();
      } catch (error) {
        console.error(error);
      }
    },
    async deleteBlog(id) {
      if (confirm('Are you sure deleting this Blog?')) {
        try {
          await blogService.deleteBlog(id);
          await this.fetchBlogs();
        } catch (error) {
          console.error(error);
        }
      }
    },
    canEdit(blog) {
      return blog.user === this.username;
    },
    canDelete(blog) {
      return this.role === 'Admin' || blog.user === this.username;
    },
    formatDate(dateStr) {
      return new Date(dateStr).toLocaleString();
    },
    getFullUrl(url) {
      if (!url) return '';
      if (url.startsWith('http')) return url;
      return `${this.backendBaseUrl}${url}`;
    },
    toggleCreateForm() {
      this.showCreateForm = !this.showCreateForm;
    }
  },
  mounted() {
    this.getTokenInfo();
    this.fetchBlogs();
    if (this.role === 'Admin') {
      this.layoutComponent = adminLayout;
    }
  }
};
</script>

<style scoped>
/* Container chính */
.container {
  padding: 20px;
  background-color: #fff0f6;
}

/* Tiêu đề trang */
.page-title {
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 1rem;
  color: #9c27b0;
  text-align: center;
}

/* Fixed icon cho New Blog */
.new-blog-icon {
  position: fixed;
  top: 70px;
  right: 10px;
  background-color: #fff;
  padding: 10px 15px;
  border-radius: 30px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.3);
  cursor: pointer;
  z-index: 1100;
  font-size: 1rem;
  color: #9c27b0;
}

/* Overlay cho form tạo blog */
.create-blog-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Nền mờ phía sau form */
.overlay-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.4);
  z-index: 1001;
}

/* Form tạo blog được căn giữa */
.create-blog-form {
  position: relative;
  z-index: 1002;
  background-color: #fce4ec;
  padding: 20px;
  border-radius: 10px;
  max-width: 600px;
  width: 90%;
  margin: 0 20px;
}

.form-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 10px;
  color: #ad1457;
  text-align: center;
}

/* Các input chung */
.input {
  width: 100%;
  padding: 10px;
  margin-bottom: 10px;
  border: 1px solid #ce93d8;
  border-radius: 6px;
  font-size: 14px;
  background-color: #f3e5f5;
}

.input-file {
  margin-bottom: 10px;
}

/* Các nút bấm */
.btn {
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  border: none;
}

.btn-primary {
  background-color: #ce93d8;
  color: white;
}

.btn-secondary {
  background-color: #f8bbd0;
  color: white;
  margin-left: 10px;
}

/* Blog List */
.blog-list {
  display: grid;
  gap: 20px;
  margin-top: 20px;
}

/* Blog Card - tone chủ đạo hồng tím */
.blog-card {
  background-color: #f3e5f5;
  border: 4px solid #ce93d8;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s, box-shadow 0.2s;
}

.blog-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 10px rgba(0, 0, 0, 0.1);
}

/* Blog Header */
.blog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.blog-meta {
  font-size: 0.875rem;
  color: #6a1b9a;
}

.author,
.time {
  font-weight: 500;
}

/* Blog Actions */
.blog-actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.25rem;
}

.edit-btn {
  color: #1976d2;
}

.delete-btn {
  color: #d32f2f;
}

/* Blog Content */
.blog-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #4a148c;
  margin-bottom: 10px;
}

.blog-content {
  color: #424242;
  white-space: pre-line;
  margin-bottom: 10px;
}

/* Blog File */
.blog-file {
  margin-bottom: 10px;
}

.blog-image {
  max-width: 100%;
  border-radius: 8px;
  border: 1px solid #ce93d8;
}

.file-link {
  color: #6a1b9a;
  text-decoration: underline;
}

/* Edit Form */
.edit-form {
  margin-top: 15px;
  border-top: 1px solid #e1bee7;
  padding-top: 15px;
}

/* Responsive */
@media (max-width: 768px) {
  .new-blog-icon {
    top: 40px;
    right: 10px;
    font-size: 0.9rem;
  }

  .create-blog-form {
    max-width: 90%;
    padding: 15px;
  }

  .form-title {
    font-size: 1rem;
  }
}

/*Changing tone for admin */

.admin-tone {
  background-color: #e3f2fd;
}

.admin-tone .page-title {
  color: #1976d2;
}

.admin-tone .new-blog-icon {
  color: #1565c0;
  background-color: #ffffff;
  border: 1px solid #bbdefb;
}

.admin-tone .create-blog-form {
  background-color: #e1f5fe;
  border: 1px solid #90caf9;
}

.admin-tone .form-title {
  color: #0d47a1;
}

.admin-tone .input {
  background-color: #e3f2fd;
  border: 1px solid #90caf9;
}

.admin-tone .btn-primary {
  background-color: #1976d2;
}

.admin-tone .blog-card {
  background-color: #e3f2fd;
  border-color: #90caf9;
}

.admin-tone .blog-title {
  color: #0d47a1;
}

.admin-tone .edit-btn {
  color: #1565c0;
}

.admin-tone .file-link {
  color: #0d47a1;
}

/* Comment tone */
.admin-tone .comments-section {
  background-color: #e3f2fd;
  border-color: #90caf9;
}

.admin-tone .comment-item {
  background-color: #ffffff;
  border-color: #bbdefb;
}

.admin-tone .comment-meta {
  color: #1976d2;
}

.admin-tone .btn-update,
.admin-tone .add-comment button {
  background-color: #1976d2;
}



/*Changing tone for admin */


@media (max-width: 480px) {
  .new-blog-icon {
    top: 20px;
    right: 10px;
    font-size: 0.85rem;
  }

  .create-blog-form {
    padding: 10px;
  }

  .form-title {
    font-size: 0.95rem;
  }
}
.search-section {
  display: flex;
  gap: 10px;
  margin: 20px 0;
  justify-content: center;
}

.search-input {
  flex: 1;
  padding: 8px;
  border: 1px solid #ce93d8;
  border-radius: 6px;
  font-size: 14px;
  min-width: 200px;
}

.search-btn,
.reset-btn {
  padding: 8px 14px;
  background-color: #ce93d8;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.reset-btn {
  background-color: #9e9e9e;
}

.search-btn:hover {
  background-color: #ba68c8;
}

.reset-btn:hover {
  background-color: #757575;
}

</style>