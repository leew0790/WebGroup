import axios from 'axios';


const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      Authorization: `Bearer ${token}`
    }
  };
};

const getAllComments = async () => {
    try {
      const response = await axios.get('https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment', getAuthHeaders());
      return response.data;
    } catch (error) {
      console.error('Error fetching comments:', error);
      throw error;
    }
  };

const createComment = async (comment) => {
    try {
      const response = await axios.post('https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment', comment, getAuthHeaders());
      return response.data;
    } catch (error) {
      console.error('Error creating comment:', error);
      throw error;
    }
};

const updateComment = async (id, comment) => {
    try {
      const response = await axios.put(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment/${id}`, comment, getAuthHeaders());
      return response.data;
    } catch (error) {
      console.error('Error updating comment:', error);
      throw error;
    }
};

const deleteComment = async (id) => {
    try {
      await axios.delete(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment/${id}`, getAuthHeaders());
      return true;
    } catch (error) {
      console.error('Error deleting comment:', error);
      return false;
    }
};

const getCommentById = async (id) => {
    try {
      const response = await axios.get(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment/${id}`,getAuthHeaders());
      return response.data;
    } catch (error) {
      console.error('Error fetching comment details:', error);
      throw error;
    }
};

const getCommentByBlogId = async (blogId) => {
    try {
      const response = await axios.get(`https://projectcomp1640-asfhatcmhzf6hghg.eastasia-01.azurewebsites.net/api/comment/blog/${blogId}`,getAuthHeaders());
      return response.data;
    } catch (error) {
      console.error('Error ', error);
      throw error;
    }
};
  






export default {
    getAllComments,
    createComment,
    getCommentById,
    updateComment,
    deleteComment,
    getCommentByBlogId
  };