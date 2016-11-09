class CreateTestEnvironments < ActiveRecord::Migration
  def change
    create_table :test_environments do |t|
      t.string :name
      t.string :environment_type
      t.string :status
      t.datetime :created_at
      t.datetime :modified_at
      t.text :config
      t.text :description

      t.timestamps
    end
  end
end
