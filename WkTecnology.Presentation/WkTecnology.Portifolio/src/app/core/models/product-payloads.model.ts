export interface CreateProductPayload {
  name: string;
  description?: string | null;
  brand: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  categoryId: number;
}

export interface UpdateProductPayload {
  name: string;
  description?: string | null;
  brand: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
}
