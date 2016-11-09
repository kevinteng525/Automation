class CreateProviders < ActiveRecord::Migration
  def change
    create_table :providers do |t|
      t.string :name
      t.integer :category
      t.string :type
      t.string :path
      t.string :config
      t.string :string
      t.string :description
      t.string :is_active
      t.string :integer

      t.timestamps
    end
  end
end
