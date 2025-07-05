export interface Category {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
  parentCategoryId?: number | null;
  parentCategoryName?: string | null;
  createdAt: Date;
  updatedAt?: Date | null;
  subCategories?: Category[];
}
