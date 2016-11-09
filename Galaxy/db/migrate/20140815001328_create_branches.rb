class CreateBranches < ActiveRecord::Migration
  def change
    create_table :branches do |t|
      t.text :name
      t.text :description
      t.text :path

      t.timestamps
    end
  end
end
