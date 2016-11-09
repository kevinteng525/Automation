class ChangeColumnUsers < ActiveRecord::Migration
  def self.up
    change_column :users, :is_active, :boolean
  end
  
  def self.down
    change_column :users, :is_active, :integer
  end
  
  def change
  end
end
