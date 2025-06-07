<template>
    <div class="comment-section">
      <h4>Bình luận</h4>
      <ul>
        <li v-for="(cmt, i) in comments" :key="i">
          {{ cmt }}
        </li>
      </ul>
      <div class="comment-input">
        <input
          type="text"
          v-model="newComment"
          placeholder="Enter comment..."
        />
        <button @click="submitComment">Gửi</button>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: "Comments",
    props: {
      // Danh sách comment từ blog
      comments: {
        type: Array,
        default: () => [],
      },
    },
    data() {
      return {
        newComment: "",
      };
    },
    methods: {
      submitComment() {
        const text = this.newComment.trim();
        if (!text) return;
        // Emit về cha -> cha tự thêm vào mảng comments của blog tương ứng
        this.$emit("add-comment", text);
        this.newComment = "";
      },
    },
  };
  </script>
  
  <style scoped>
  .comment-section {
    margin-top: 10px;
    padding-top: 10px;
    border-top: 1px solid #ddd;
  }
  
  .comment-section h4 {
    margin: 0 0 10px 0;
  }
  
  .comment-section ul {
    list-style: none;
    padding-left: 0;
    margin: 0 0 10px 0;
  }
  
  .comment-section li {
    background: #fff;
    border: 1px solid #ddd;
    padding: 6px;
    margin-bottom: 5px;
    border-radius: 4px;
  }
  
  .comment-input {
    display: flex;
    gap: 8px;
  }
  
  .comment-input input {
    flex: 1;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
  }
  
  .comment-input button {
    padding: 8px 16px;
    border: none;
    background-color: #0275d8;
    color: #fff;
    border-radius: 4px;
    cursor: pointer;
  }
  </style>