class CreateTestCases < ActiveRecord::Migration
  def change
    create_table :test_cases do |t|
      t.integer :source_id
      t.string :name
      t.integer :product_id
      t.string :feature
      t.string :script_path
      t.integer :weight
      t.boolean :is_automated
      t.integer :created_by
      t.integer :modified_by
      t.text :description

      t.timestamps
    end
  end
end
