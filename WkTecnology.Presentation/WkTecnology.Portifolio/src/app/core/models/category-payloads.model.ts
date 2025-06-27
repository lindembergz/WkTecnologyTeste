export interface CreateCategoryPayload {
  name: string;
  description?: string | null;
  isActive: boolean;
  parentCategoryId?: number | null;
}

export interface UpdateCategoryPayload {
  name: string;
  description?: string | null;
  isActive: boolean;
  parentCategoryId?: number | null;
}
