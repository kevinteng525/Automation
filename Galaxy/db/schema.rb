# encoding: UTF-8
# This file is auto-generated from the current state of the database. Instead
# of editing this file, please use the migrations feature of Active Record to
# incrementally modify your database, and then regenerate this schema definition.
#
# Note that this schema.rb definition is the authoritative source for your
# database schema. If you need to create the application database on another
# system, you should be using db:schema:load, not running all the migrations
# from scratch. The latter is a flawed and unsustainable approach (the more migrations
# you'll amass, the slower it'll run and the greater likelihood for issues).
#
# It's strongly recommended that you check this file into your version control system.

ActiveRecord::Schema.define(version: 20141014023254) do

  create_table "automation_jobs", force: true do |t|
    t.text     "name"
    t.integer  "sut_environment_id"
    t.integer  "test_agent_environment_id"
    t.integer  "job_type"
    t.integer  "priority"
    t.integer  "status"
    t.integer  "retry_times"
    t.integer  "time_out"
    t.integer  "create_by"
    t.integer  "modify_by"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "automation_task_running_statuses", force: true do |t|
    t.integer  "task_id"
    t.integer  "status"
    t.text     "information"
    t.integer  "execution_percentage"
    t.string   "result_type_list"
    t.string   "result_count_list"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "automation_tasks", force: true do |t|
    t.string   "name"
    t.integer  "status"
    t.integer  "task_type"
    t.integer  "priority"
    t.datetime "create_date"
    t.integer  "create_by"
    t.datetime "modify_date"
    t.integer  "modify_by"
    t.integer  "build_id"
    t.integer  "supported_environment_id"
    t.string   "test_content"
    t.text     "information"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.integer  "recurrence_pattern"
    t.text     "start_date"
    t.text     "start_time"
    t.integer  "week_days"
    t.integer  "week_interval"
    t.integer  "product_id"
    t.integer  "branch_id"
    t.integer  "release_id"
    t.integer  "parent_task_id"
  end

  create_table "branches", force: true do |t|
    t.text     "name"
    t.text     "description"
    t.text     "path"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.integer  "product_id"
  end

  create_table "builds", force: true do |t|
    t.integer  "product_id"
    t.string   "name"
    t.string   "build_type"
    t.string   "status"
    t.string   "branch"
    t.string   "number"
    t.string   "location"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "diagnostic_logs", force: true do |t|
    t.datetime "create_time"
    t.string   "component"
    t.integer  "log_type"
    t.text     "message"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "product_supported_environment_maps", force: true do |t|
    t.integer  "product_id"
    t.integer  "supported_environment_id"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "products", force: true do |t|
    t.string   "name"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "projects", force: true do |t|
    t.text     "name"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "providers", force: true do |t|
    t.string   "name"
    t.integer  "category"
    t.string   "type"
    t.string   "path"
    t.string   "config"
    t.string   "string"
    t.string   "description"
    t.string   "is_active"
    t.string   "integer"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "rankings", force: true do |t|
    t.text     "name"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "releases", force: true do |t|
    t.text     "name"
    t.text     "description"
    t.text     "path"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.integer  "product_id"
  end

  create_table "sessions", force: true do |t|
    t.string   "session_id", null: false
    t.text     "data"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  add_index "sessions", ["session_id"], name: "index_sessions_on_session_id", unique: true
  add_index "sessions", ["updated_at"], name: "index_sessions_on_updated_at"

  create_table "subscribers", force: true do |t|
    t.integer  "product_id"
    t.integer  "user_id"
    t.datetime "create_time"
    t.text     "description"
    t.integer  "subscriber_type"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "supported_environments", force: true do |t|
    t.string   "name"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.string   "config"
  end

  create_table "test_case_executions", force: true do |t|
    t.integer  "test_case_id"
    t.integer  "job_id"
    t.integer  "status"
    t.datetime "start_time"
    t.datetime "end_time"
    t.integer  "retry_times"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.text     "info"
  end

  create_table "test_cases", force: true do |t|
    t.integer  "source_id"
    t.string   "name"
    t.integer  "product_id"
    t.string   "feature"
    t.string   "script_path"
    t.integer  "weight"
    t.boolean  "is_automated"
    t.integer  "created_by"
    t.integer  "modified_by"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

  create_table "test_environments", force: true do |t|
    t.string   "name"
    t.string   "environment_type"
    t.string   "status"
    t.datetime "created_at"
    t.datetime "modified_at"
    t.text     "config"
    t.text     "description"
    t.datetime "updated_at"
  end

  create_table "test_results", force: true do |t|
    t.integer  "execution_id"
    t.integer  "result"
    t.boolean  "is_triaged"
    t.integer  "triaged_by"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
    t.string   "files"
  end

  create_table "test_suites", force: true do |t|
    t.string   "name"
    t.string   "sub_suites"
    t.string   "test_cases"
    t.integer  "created_by"
    t.integer  "modified_by"
    t.text     "description"
    t.boolean  "is_root",     default: false
    t.datetime "created_at"
    t.datetime "updated_at"
    t.integer  "suite_type"
  end

  create_table "users", force: true do |t|
    t.string   "name"
    t.string   "password"
    t.integer  "user_type"
    t.integer  "role"
    t.integer  "is_active"
    t.text     "description"
    t.datetime "created_at"
    t.datetime "updated_at"
  end

end
