class CreateTestResults < ActiveRecord::Migration
  def change
    create_table :test_results do |t|
      t.integer :execution_id
      t.integer :result
      t.boolean :is_triaged
      t.integer :triaged_by
      t.text :description

      t.timestamps
    end
  end
end
