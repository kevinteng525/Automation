class CreateReleases < ActiveRecord::Migration
  def change
    create_table :releases do |t|
      t.text :name
      t.text :description
      t.text :path

      t.timestamps
    end
  end
end
