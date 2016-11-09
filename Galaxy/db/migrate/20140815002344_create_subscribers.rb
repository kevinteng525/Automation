class CreateSubscribers < ActiveRecord::Migration
  def change
    create_table :subscribers do |t|
      t.integer :product_id
      t.integer :user_id
      t.datetime :create_time
      t.text :description
      t.integer :subscriber_type

      t.timestamps
    end
  end
end
