class CreateProductSupportedEnvironmentMaps < ActiveRecord::Migration
  def change
    create_table :product_supported_environment_maps do |t|
      t.integer :product_id
      t.integer :supported_environment_id

      t.timestamps
    end
  end
end
