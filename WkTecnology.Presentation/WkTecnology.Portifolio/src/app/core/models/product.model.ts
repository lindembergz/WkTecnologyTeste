export interface Product {
  id: number;
  name: string;
  description?: string | null;
  brand: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  isActive: boolean;
  categoryId: number;
  categoryName: string;
  createdAt: Date;
  updatedAt?: Date | null;
}
