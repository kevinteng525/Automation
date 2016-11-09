class AddProductIdToRelease < ActiveRecord::Migration
  def change
    add_column :releases, :product_id, :integer
  end
end
