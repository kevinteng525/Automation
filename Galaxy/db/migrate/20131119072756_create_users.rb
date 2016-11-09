class CreateUsers < ActiveRecord::Migration
  def change
    create_table :users do |t|
      t.string :name
      t.string :password
      t.integer :user_type
      t.integer :role
      t.integer :is_active
      t.text :description

      t.timestamps
    end
  end
end
