class CreateDiagnosticLogs < ActiveRecord::Migration
  def change
    create_table :diagnostic_logs do |t|
      t.datetime :create_time
      t.string :component
      t.integer :log_type
      t.text :message

      t.timestamps
    end
  end
end
