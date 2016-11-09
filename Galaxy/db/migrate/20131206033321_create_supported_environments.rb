class CreateSupportedEnvironments < ActiveRecord::Migration
  def change
    create_table :supported_environments do |t|
      t.string :name
      t.text :description

      t.timestamps
    end
  end
end
