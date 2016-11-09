class CreateTestSuites < ActiveRecord::Migration
  def change
    create_table :test_suites do |t|
      t.string :name
      t.string :sub_suites
      t.string :test_cases
      t.integer :created_by
      t.integer :modified_by
      t.text :description
      t.boolean :is_root, :default=>false

      t.timestamps
    end
  end
end
