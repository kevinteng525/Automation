require 'test_helper'

class TestEnvironmentsControllerTest < ActionController::TestCase
  setup do
    @test_environment = test_environments(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:test_environments)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create test_environment" do
    assert_difference('TestEnvironment.count') do
      post :create, test_environment: { config: @test_environment.config, created_at: @test_environment.created_at, description: @test_environment.description, environment_type: @test_environment.environment_type, modified_at: @test_environment.modified_at, name: @test_environment.name, status: @test_environment.status }
    end

    assert_redirected_to test_environment_path(assigns(:test_environment))
  end

  test "should show test_environment" do
    get :show, id: @test_environment
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @test_environment
    assert_response :success
  end

  test "should update test_environment" do
    patch :update, id: @test_environment, test_environment: { config: @test_environment.config, created_at: @test_environment.created_at, description: @test_environment.description, environment_type: @test_environment.environment_type, modified_at: @test_environment.modified_at, name: @test_environment.name, status: @test_environment.status }
    assert_redirected_to test_environment_path(assigns(:test_environment))
  end

  test "should destroy test_environment" do
    assert_difference('TestEnvironment.count', -1) do
      delete :destroy, id: @test_environment
    end

    assert_redirected_to test_environments_path
  end
end
