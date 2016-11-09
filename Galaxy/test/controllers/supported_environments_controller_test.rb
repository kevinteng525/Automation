require 'test_helper'

class SupportedEnvironmentsControllerTest < ActionController::TestCase
  setup do
    @supported_environment = supported_environments(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:supported_environments)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create supported_environment" do
    assert_difference('SupportedEnvironment.count') do
      post :create, supported_environment: { description: @supported_environment.description, name: @supported_environment.name }
    end

    assert_redirected_to supported_environment_path(assigns(:supported_environment))
  end

  test "should show supported_environment" do
    get :show, id: @supported_environment
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @supported_environment
    assert_response :success
  end

  test "should update supported_environment" do
    patch :update, id: @supported_environment, supported_environment: { description: @supported_environment.description, name: @supported_environment.name }
    assert_redirected_to supported_environment_path(assigns(:supported_environment))
  end

  test "should destroy supported_environment" do
    assert_difference('SupportedEnvironment.count', -1) do
      delete :destroy, id: @supported_environment
    end

    assert_redirected_to supported_environments_path
  end
end
