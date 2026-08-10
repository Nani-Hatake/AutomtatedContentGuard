export type SubmissionStatus = 'Approved' | 'Flagged';

export interface ContentSubmission {
  id: number;
  textContent: string;
  toxicityScore: number;
  status: SubmissionStatus;
  isFlagged: boolean;
  submittedAt: string;
}

export interface ForbiddenWord {
  id: number;
  word: string;
}
