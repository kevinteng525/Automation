class CreateBuilds < ActiveRecord::Migration
  def change
    create_table :builds do |t|
      t.integer :product_id
      t.string :name
      t.string :build_type
      t.string :status
      t.string :branch
      t.string :number
      t.string :location
      t.text :description

      t.timestamps
    end
  end
end
