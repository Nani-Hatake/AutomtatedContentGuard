import axios from 'axios';
import type { ContentSubmission, ForbiddenWord } from '../types';

// Hardcode the Render URL directly to bypass any Vercel env variable build issues
const API_BASE_URL = 'https://automtatedcontentguard.onrender.com/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 20000, // 20s to handle Render free-tier cold starts
  headers: {
    'Content-Type': 'application/json',
  },
});

const handleAxiosError = (error: unknown): never => {
  if (axios.isAxiosError(error)) {
    if (error.code === 'ECONNABORTED') {
      throw new Error('Request timed out. The backend might be waking up from sleep mode, please try again.');
    }
    if (error.response) {
      const responseData = error.response.data as Record<string, unknown> | null;
      const messageFromData = responseData && typeof responseData === 'object'
        ? (responseData.message as string | undefined) || (responseData.title as string | undefined) || JSON.stringify(responseData)
        : null;
      const message = typeof messageFromData === 'string'
        ? messageFromData
        : `Backend responded with status ${error.response.status}.`;
      throw new Error(message);
    }
    throw new Error('Unable to reach the backend. Please verify your connection.');
  }

  throw new Error('Unexpected API error.');
};

export const fetchSubmissions = async (): Promise<ContentSubmission[]> => {
  try {
    const response = await api.get<ContentSubmission[]>('/ContentSubmissions');
    return response.data;
  } catch (error) {
    return handleAxiosError(error);
  }
};

export const postSubmission = async (textContent: string): Promise<ContentSubmission> => {
  try {
    const response = await api.post<ContentSubmission>('/ContentSubmissions', { textContent });
    return response.data;
  } catch (error) {
    return handleAxiosError(error);
  }
};

export const fetchForbiddenWords = async (): Promise<ForbiddenWord[]> => {
  try {
    const response = await api.get<ForbiddenWord[]>('/ForbiddenWords');
    return response.data;
  } catch (error) {
    return handleAxiosError(error);
  }
};

export const addForbiddenWord = async (word: string, severityScore = 5): Promise<ForbiddenWord> => {
  try {
    const response = await api.post<ForbiddenWord>('/ForbiddenWords', { word, severityScore });
    return response.data;
  } catch (error) {
    return handleAxiosError(error);
  }
};

export const deleteForbiddenWord = async (id: number): Promise<void> => {
  try {
    await api.delete(`/ForbiddenWords/${id}`);
  } catch (error) {
    handleAxiosError(error);
  }
};
