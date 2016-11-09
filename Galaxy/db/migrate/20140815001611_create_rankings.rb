class CreateRankings < ActiveRecord::Migration
  def change
    create_table :rankings do |t|
      t.text :name
      t.text :description

      t.timestamps
    end
  end
end
