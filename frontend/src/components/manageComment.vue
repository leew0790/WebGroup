<template>
    <div :class="['comments-section', { 'admin-tone': isAdmin }]">
      <h4 class="comments-title">Comments</h4>
  
      <!-- Danh sách comment, hiển thị tối đa theo visibleComments -->
      <div v-for="(comment, index) in visibleComments" :key="comment.id" class="comment-item">
        <!-- Header comment: Tác giả + Thời gian + Nút Edit/Delete -->
        <div class="comment-header">
          <div class="comment-meta">
            By: <strong>{{ comment.userFullName }}</strong> - {{ formatDate(comment.createdOn) }}
          </div>
          <!-- Chỉ hiển thị Edit/Delete nếu user có quyền -->
          <div class="comment-actions">
            <button
                v-if="canEditComment(comment)"
                @click="startEditComment(index)"
                class="action-btn edit-btn"
                title="Edit Comment"
            >
                ✏️
            </button>
            <button
                v-if="canDeleteComment(comment)"
                @click="deleteComment(comment.id)"
                class="action-btn delete-btn"
                title="Delete Comment"
            >
                🗑️
            </button>
            </div>
        </div>
  
        
        <div v-if="editingCommentIndex === index" class="edit-form">
          <textarea v-model="editCommentContent" rows="3"></textarea>
          <div class="edit-form-actions">
            <button @click="updateComment(comment.id)" class="btn-update">Update</button>
            <button @click="cancelEdit" class="btn-cancel">Cancel</button>
          </div>
        </div>
        <p v-else class="comment-content">{{ comment.content }}</p>
      </div>
  
      <!-- Nút Show more / Show less -->
      <div v-if="comments.length > 3" class="toggle-comments">
        <button v-if="maxVisibleComments < comments.length" @click="showAllComments">
          Show more
        </button>
        <button v-else @click="showLessComments">
          Show less
        </button>
      </div>
  
      <!-- Form thêm comment mới -->
      <div v-if="currentUserRole !== 'Admin'" class="add-comment">
        <textarea
          v-model="newComment"
          placeholder="Comment..."
          rows="2"
        ></textarea>
        <button @click="createComment">Post Comment</button>
      </div>
    </div>
  </template>
  
  <script>
  import commentService from '../api/commentService';
  import jwtDecode from 'jwt-decode';
  
  export default {
    name: 'Comments',
    props: {
      blogId: {
        type: Number,
        required: true
      },
      isAdmin: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        comments: [],
        newComment: '',
        editingCommentIndex: null,
        editCommentContent: '',
        maxVisibleComments: 3,
        currentUserId: null,
        currentUserRole: null
      };
    },
    computed: {
      visibleComments() {
        return this.comments.slice(0, this.maxVisibleComments);
      }
    },
    created() {
      const token = localStorage.getItem('token');
      if (token) {
        const decoded = jwtDecode(token);
        this.currentUserId = decoded.userId || decoded.nameid;
        this.currentUserRole = decoded.role;
      }
      this.fetchComments();
    },
    methods: {

        canEditComment(comment) {
            return this.currentUserRole !== 'Admin' && comment.userId === this.currentUserId;
        },
        // Cho phép xóa nếu người dùng là tác giả hoặc là admin
        canDeleteComment(comment) {
            return comment.userId === this.currentUserId || this.currentUserRole === 'Admin';
        },


      async fetchComments() {
        try {
          const res = await commentService.getCommentByBlogId(this.blogId);
          this.comments = res;
        } catch (error) {
          console.error('Error fetching comments:', error);
        }
      },
      async createComment() {
        try {
          const commentData = {
            content: this.newComment,
            blogId: this.blogId
          };
          await commentService.createComment(commentData);
          this.newComment = '';
          this.fetchComments();
        } catch (error) {
          console.error('Error creating comment:', error);
        }
      },
      startEditComment(index) {
        this.editingCommentIndex = index;
        this.editCommentContent = this.comments[index].content;
      },
      cancelEdit() {
        this.editingCommentIndex = null;
        this.editCommentContent = '';
      },
      async updateComment(commentId) {
        try {
          const updateData = { content: this.editCommentContent };
          await commentService.updateComment(commentId, updateData);
          this.fetchComments();
          this.cancelEdit();
        } catch (error) {
          console.error('Error updating comment:', error);
        }
      },
      async deleteComment(commentId) {
        try {
          if (confirm('Are you sure deleting this commment?')) {
            await commentService.deleteComment(commentId);
            this.fetchComments();
          }
        } catch (error) {
          console.error('Error deleting comment:', error);
        }
      },
      formatDate(dateStr) {
        const date = new Date(dateStr);
        return date.toLocaleString();
      },
      
      showAllComments() {
        this.maxVisibleComments = this.comments.length;
      },
      showLessComments() {
        this.maxVisibleComments = 3;
      }
    }
  };
  </script>
  
  <style scoped>
  /* Khu vực comment chính */
  .comments-section {
    margin-top: 20px;
    padding: 15px;
    background-color: #fff0f6; /* Hồng nhạt */
    border: 1px solid #f8bbd0;
    border-radius: 8px;
  }
  
  /* Tiêu đề Comments */
  .comments-title {
    font-size: 1.25rem;
    margin-bottom: 15px;
    color: #ad1457;
    font-weight: 600;
  }
  
  /* Mỗi comment */
  .comment-item {
    background-color: #fdf3f8; /* hồng nhạt hơn để tách biệt */
    border: 1px solid #f8bbd0;
    border-radius: 8px;
    padding: 10px;
    margin-bottom: 15px;
    position: relative;
  }
  
  /* Header comment: meta + actions */
  .comment-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 5px;
  }
  
  /* Thông tin tác giả, thời gian */
  .comment-meta {
    font-size: 0.875rem;
    color: #6a1b9a;
  }
  
  /* Nút Edit / Delete */
  .comment-actions {
    display: flex;
    gap: 10px;
  }
  .action-btn {
    background: none;
    border: none;
    cursor: pointer;
    font-size: 1rem;
    padding: 4px;
    border-radius: 4px;
    transition: background-color 0.2s;
  }
  .action-btn:hover {
    background-color: #f3e5f5; /* hover nhẹ */
  }
  .edit-btn {
    color: #1976d2;
  }
  .delete-btn {
    color: #d32f2f;
  }
  
  /* Nội dung comment */
  .comment-content {
    margin: 0;
    font-size: 1rem;
    color: #333;
  }
  
  /* Form edit comment */
  .edit-form textarea {
    width: 100%;
    padding: 8px;
    border: 1px solid #ce93d8;
    border-radius: 4px;
    resize: vertical;
    margin-bottom: 10px;
  }
  .edit-form-actions {
    display: flex;
    gap: 10px;
  }
  .btn-update,
  .btn-cancel {
    padding: 6px 12px;
    border-radius: 4px;
    border: none;
    cursor: pointer;
  }
  .btn-update {
    background-color: #ce93d8;
    color: #fff;
  }
  .btn-cancel {
    background-color: #f8bbd0;
    color: #fff;
  }
  
  /* Khu vực Show more/less */
  .toggle-comments {
    text-align: center;
    margin-bottom: 15px;
  }
  .toggle-comments button {
    padding: 6px 12px;
    font-size: 0.9rem;
    cursor: pointer;
    background-color: #f8bbd0;
    color: #fff;
    border: none;
    border-radius: 4px;
  }
  
  /* Form thêm comment */
  .add-comment {
    margin-top: 15px;
  }
  .add-comment textarea {
    width: 100%;
    padding: 8px;
    border-radius: 4px;
    border: 1px solid #ce93d8;
    background-color: #fdf3f8;
    resize: vertical;
    margin-bottom: 10px;
  }
  .add-comment button {
    padding: 8px 16px;
    background-color: #ce93d8;
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }

/*changing for admin */

 
.admin-tone {
  background-color: #e3f2fd;
  border: 1px solid #90caf9;
}

.admin-tone .comments-title {
  color: #0d47a1;
}

.admin-tone .comment-item {
  background-color: #eaf4fd;
  border: 1px solid #64b5f6;
}

.admin-tone .comment-meta {
  color: #1565c0;
}

.admin-tone .action-btn:hover {
  background-color: #bbdefb;
}

.admin-tone .edit-form textarea,
.admin-tone .add-comment textarea {
  border: 1px solid #64b5f6;
  background-color: #f1f8ff;
}

.admin-tone .btn-update,
.admin-tone .add-comment button {
  background-color: #1976d2;
  color: white;
}

.admin-tone .btn-cancel {
  background-color: #90caf9;
  color: white;
}

.admin-tone .toggle-comments button {
  background-color: #64b5f6;
  color: white;
}
/*changing for admin */
  </style>
  