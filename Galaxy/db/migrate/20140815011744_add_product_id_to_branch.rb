class AddProductIdToBranch < ActiveRecord::Migration
  def change
    add_column :branches, :product_id, :integer
  end
end
