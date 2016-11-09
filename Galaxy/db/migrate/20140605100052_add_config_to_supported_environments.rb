class AddConfigToSupportedEnvironments < ActiveRecord::Migration
  def change
    add_column :supported_environments, :config, :string
  end
end
